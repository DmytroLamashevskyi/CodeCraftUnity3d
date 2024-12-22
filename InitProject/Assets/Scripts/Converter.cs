using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homework
{
    /**
       Конвертер представляет собой преобразователь ресурсов, который берет ресурсы
       из зоны погрузки (справа) и через несколько секунд преобразовывает его в
       ресурсы другого типа (слева).
       
       Конвертер работает автоматически. Когда заканчивается цикл переработки
       ресурсов, то конвертер берет следующую партию и начинает цикл по новой, пока
       можно брать ресурсы из зоны загрузки или пока есть место для ресурсов выгрузки.
       
       Также конвертер можно выключать. Если конвертер во время работы был
       выключен, то он возвращает обратно ресурсы в зону загрузки. Если в это время
       были добавлены еще ресурсы, то при переполнении возвращаемые ресурсы
       «сгорают».
     
       Характеристики конвертера:
       - Зона погрузки: вместимость бревен
       - Зона выгрузки: вместимость досок
       - Кол-во ресурсов, которое берется с зоны погрузки
       - Кол-во ресурсов, которое поставляется в зону выгрузки
       - Время преобразования ресурсов
       - Состояние: вкл/выкл
     */ 
    public interface IConverter
    {
        /// <summary>
        /// Событие завершения обработки
        /// </summary>
        event Action<Resource> ProcessingComplete;

        /// <summary>
        /// Событие переполнения зоны
        /// </summary>
        event Action<string> ZoneOverflow;

        /// <summary>
        /// Включить и начать брать ресурсы из зоны загрузки
        /// </summary>
        void Enable();

        /// <summary>
        /// Выключить и прервать текущую конвертацию
        /// </summary>
        void Disable();

        /// <summary>
        /// Проверить, включен ли конвертер
        /// </summary>
        /// <returns>True, если конвертер включен</returns>
        bool IsEnabled();

        // Управление процессом обработки
        /// <summary>
        /// Проверить, идет ли процесс обработки
        /// </summary>
        /// <returns>True, если идет обработка</returns>
        bool IsProcessing();

        /// <summary>
        /// Получить оставшееся время до завершения текущей обработки
        /// </summary>
        /// <returns>Время в секундах</returns>
        float TimeLeft();

        /// <summary>
        /// Добавить рессурсы в склад загрузки
        /// </summary>
        /// <param name="resource">Тип ресурса</param>
        /// <param name="amount">Количество </param>
        void AddResourcesLoadZone(Resource resource, int amount);

        /// <summary>
        /// Забрать ресурсы из склада загрузки
        /// </summary>
        /// <param name="resource">Тип рессурса</param>
        /// <param name="amount">Количество</param>
        void PullResourcesLoadZone(Resource resource, int amount);

        /// <summary>
        /// Забирает рессурсы из конвертированного склада
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="amount"></param>
        void PullResourcesUnloadZone(Resource resource, int amount);
        /// <summary>
        /// Правила конвертации для конвеера
        /// </summary>
        /// <param name="rule">Правило</param>
        void SetConversionRule(ConversionRule rule);
        /// <summary>
        /// Информация о свободном месте на зоне погрузки
        /// </summary>
        /// <returns>Общая вместительность</returns>
        int GetLoadZoneCapacity();
        /// <summary>
        /// Информация сколько свободного места осталось
        /// </summary>
        /// <returns>Свободное место</returns>
        int GetFreeLoadZoneCapacity(); 
        /// <summary>
        /// Информация о свободном месте на зоне выгрузки
        /// </summary>
        /// <returns>Общая вместительность</returns>
        int GetUnloadZoneCapacity();
        /// <summary>
        /// Информация сколько свободного места осталось в зоне выгрузки
        /// </summary>
        /// <returns>Свободное место</returns>
        int GetFreeUnloadZoneCapacity();
        /// <summary>
        /// Получить список ресурсов и количесвто 
        /// </summary>
        /// <returns>Ресурсы зоны погрузки</returns>
        Dictionary<Resource, int> GetUnloadZoneResources(); 
        /// <summary>
        /// Получить список ресурсов и количесвто 
        /// </summary>
        /// <returns>Ресурсы зоны выгрузки</returns>
        Dictionary<Resource, int> GetLoadZoneResources();
    }

    public sealed class Converter : IConverter
    {
        public event Action<Resource> ProcessingComplete;
        public event Action<string> ZoneOverflow;

        private bool _isEnabled;
        private bool _isProcessing;
        private float _timeLeft;
        private int _loadZoneCapacity;
        private int _unloadZoneCapacity;
        private Dictionary<Resource, int> _loadZoneResources;
        private Dictionary<Resource, int> _unloadZoneResources;
        private List<ConversionRule> _conversionRules;

        public Converter(int loadZoneCapacity, int unloadZoneCapacity)
        {
            if(loadZoneCapacity <= 0)
                throw new ArgumentException("Load zone capacity must be greater than 0.", nameof(loadZoneCapacity));

            if(unloadZoneCapacity <= 0)
                throw new ArgumentException("Unload zone capacity must be greater than 0.", nameof(unloadZoneCapacity));

            _isEnabled = false;
            _isProcessing = false;
            _timeLeft = 0f;
            _loadZoneCapacity = loadZoneCapacity;
            _unloadZoneCapacity = unloadZoneCapacity;
            _loadZoneResources = new Dictionary<Resource, int>();
            _unloadZoneResources = new Dictionary<Resource, int>();
            _conversionRules = new List<ConversionRule>();

            // Инициализация зон ресурсов пустыми значениями
            foreach(Resource resource in Enum.GetValues(typeof(Resource)))
            {
                _loadZoneResources[resource] = 0;
                _unloadZoneResources[resource] = 0;
            }
        }
        public void Enable()
        {
            if(_isEnabled)
                return;

            _isEnabled = true;

            // Если ресурсы есть в зоне загрузки и есть место в зоне выгрузки, начинаем обработку
            if(_loadZoneResources.Any(resource => resource.Value > 0) &&
                _unloadZoneResources.Values.Sum() < _unloadZoneCapacity)
            {
                StartProcessing();
            }
        }

        public void Disable()
        {
            if(!_isEnabled)
                return;

            _isEnabled = false;
            _isProcessing = false;
            _timeLeft = 0f;
        }

        public bool IsEnabled() => _isEnabled; 
        public bool IsProcessing() => _isProcessing; 
        public float TimeLeft() => _isProcessing ? _timeLeft : 0f; 
        public int GetLoadZoneCapacity() => _loadZoneCapacity;
        public int GetFreeLoadZoneCapacity() => _loadZoneCapacity - _loadZoneResources.Values.Sum();
        public int GetUnloadZoneCapacity() => _unloadZoneCapacity;
        public int GetFreeUnloadZoneCapacity() => _unloadZoneCapacity - _unloadZoneResources.Values.Sum();


        public void AddResourcesLoadZone(Resource resource, int amount)
        {
            if(amount <= 0)
                throw new ArgumentException("Amount must be greater than 0.", nameof(amount));

            if(!_loadZoneResources.ContainsKey(resource))
                throw new InvalidOperationException($"Resource type {resource} is not supported in the load zone.");

            int freeCapacity = GetFreeLoadZoneCapacity();

            if(amount > freeCapacity)
            {
                _loadZoneResources[resource] += freeCapacity;
                ZoneOverflow?.Invoke("Load Zone");
            }
            else
            {
                _loadZoneResources[resource] += amount;
            }
        } 

        public void PullResourcesLoadZone(Resource resource, int amount)
        {
            if(amount <= 0)
                throw new ArgumentException("Amount must be greater than 0.", nameof(amount));

            if(!_loadZoneResources.ContainsKey(resource) || _loadZoneResources[resource] < amount)
                throw new InvalidOperationException($"Not enough {resource} in load zone to pull.");

            _loadZoneResources[resource] -= amount;
        }

        public void SetConversionRule(ConversionRule rule)
        {
            if(rule == null)
                throw new ArgumentNullException(nameof(rule), "Conversion rule cannot be null.");

            if(rule.InputResource == rule.OutputResource)
                throw new ArgumentException("Input resource and output resource cannot be the same.", nameof(rule));

            if(rule.InputAmount <= 0)
                throw new ArgumentException("Input amount must be greater than 0.", nameof(rule));

            if(rule.OutputAmount <= 0)
                throw new ArgumentException("Output amount must be greater than 0.", nameof(rule));

            if(rule.Duration <= 0)
                throw new ArgumentException("Duration must be greater than 0.", nameof(rule));

            _conversionRules.Clear();
            _conversionRules.Add(rule);
        }

        public void PullResourcesUnloadZone(Resource resource, int amount)
        {
            if(amount <= 0)
                throw new ArgumentException("Amount must be greater than 0.", nameof(amount));

            if(!_unloadZoneResources.ContainsKey(resource) || _unloadZoneResources[resource] < amount)
                throw new InvalidOperationException($"Not enough {resource} in unload zone to pull.");

            _unloadZoneResources[resource] -= amount;

            if(_unloadZoneResources[resource] == 0)
            {
                _unloadZoneResources.Remove(resource);
            }
        }

        public Dictionary<Resource, int> GetUnloadZoneResources()
        {
            return new Dictionary<Resource, int>(_unloadZoneResources);
        }

        public Dictionary<Resource, int> GetLoadZoneResources()
        {
            return new Dictionary<Resource, int>(_loadZoneResources);
        }

        private void StartProcessing()
        {
            if(!_isEnabled || _isProcessing)
                return;

            var rule = _conversionRules.FirstOrDefault();
            if(rule == null)
                return;

            if(!_loadZoneResources.ContainsKey(rule.InputResource) || _loadZoneResources[rule.InputResource] < rule.InputAmount)
                return;

            int freeUnloadCapacity = GetFreeUnloadZoneCapacity();
            if(freeUnloadCapacity < rule.OutputAmount)
            {
                ZoneOverflow?.Invoke("Unload Zone");
                return;
            }

            _isProcessing = true;
            _timeLeft = rule.Duration;

            DateTime startTime = DateTime.UtcNow;
            DateTime endTime = startTime.AddSeconds(rule.Duration);

            Task.Run(async () =>
            {
                while(DateTime.UtcNow < endTime && _isEnabled)
                {
                    await Task.Delay(100);
                    _timeLeft = (float)(endTime - DateTime.UtcNow).TotalSeconds;
                    if(_timeLeft < 0) _timeLeft = 0;
                }

                if(_isEnabled)
                {
                    CompleteProcessing(rule);
                }
                else
                {
                    _isProcessing = false; // Обработка была прервана
                }
            });
        }


        private void CompleteProcessing(ConversionRule rule)
        {
            // Уменьшаем входные ресурсы
            _loadZoneResources[rule.InputResource] -= rule.InputAmount;
            if(_loadZoneResources[rule.InputResource] == 0)
                _loadZoneResources.Remove(rule.InputResource);

            // Добавляем выходные ресурсы
            if(!_unloadZoneResources.ContainsKey(rule.OutputResource))
                _unloadZoneResources[rule.OutputResource] = 0;
            _unloadZoneResources[rule.OutputResource] += rule.OutputAmount;

            // Сбрасываем состояние обработки
            _isProcessing = false;
            _timeLeft = 0f;

            // Вызываем событие завершения обработки
            ProcessingComplete?.Invoke(rule.OutputResource);

            // Запускаем следующую обработку, если есть ресурсы
            StartProcessing();
        }

    }
}