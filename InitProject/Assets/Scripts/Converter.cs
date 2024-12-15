using System;
using System.Collections.Generic;

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


    }
}