using NUnit.Framework;
using System;

namespace Homework
{
    [TestFixture]
    public class ConverterTests
    {
        // Инициализация
        [Test]
        public void ShouldInitializeConverterWithCorrectCapacities()
        {
            // Arrange
            int loadZoneCapacity = 10;
            int unloadZoneCapacity = 15;

            // Act
            IConverter converter = new Converter(loadZoneCapacity, unloadZoneCapacity);

            // Assert
            Assert.AreEqual(loadZoneCapacity, converter.GetLoadZoneCapacity(), "Load zone capacity should match the input value.");
            Assert.AreEqual(unloadZoneCapacity, converter.GetUnloadZoneCapacity(), "Unload zone capacity should match the input value.");
        }

        [Test]
        public void ShouldThrowExceptionWhenInitializingWithInvalidCapacities()
        {
            // Arrange
            int invalidLoadZoneCapacity = -1;
            int invalidUnloadZoneCapacity = 0;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Converter(invalidLoadZoneCapacity, 10),
                "Constructor should throw ArgumentException when load zone capacity is invalid.");
            Assert.Throws<ArgumentException>(() => new Converter(10, invalidUnloadZoneCapacity),
                "Constructor should throw ArgumentException when unload zone capacity is invalid.");
        }

        // События
        [Test]
        public void ShouldTriggerProcessingCompleteEventWhenProcessingFinishes()
        {
            // Тест на срабатывание события ProcessingComplete
        }

        [Test]
        public void ShouldTriggerZoneOverflowEventWhenZoneIsOverflowing()
        {
            // Тест на срабатывание события ZoneOverflow при переполнении зоны
        }

        // Управление состоянием
        [Test]
        public void ShouldEnableConverter()
        {
            // Тест на успешное включение конвертера
        }

        [Test]
        public void ShouldDisableConverter()
        {
            // Тест на успешное выключение конвертера
        }

        [Test]
        public void ShouldReturnTrueWhenConverterIsEnabled()
        {
            // Тест на проверку, что конвертер включен
        }

        [Test]
        public void ShouldReturnFalseWhenConverterIsDisabled()
        {
            // Тест на проверку, что конвертер выключен
        }

        // Управление процессом обработки
        [Test]
        public void ShouldReturnTrueWhenProcessing()
        {
            // Тест на проверку, что процесс обработки идет
        }

        [Test]
        public void ShouldReturnFalseWhenNotProcessing()
        {
            // Тест на проверку, что процесс обработки не идет
        }

        [Test]
        public void ShouldReturnCorrectTimeLeftForProcessing()
        {
            // Тест на получение корректного оставшегося времени обработки
        }

        // Работа с ресурсами в зоне загрузки
        [Test]
        public void ShouldAddResourcesToLoadZone()
        {
            // Тест на успешное добавление ресурсов в зону загрузки
        }

        [Test]
        public void ShouldNotExceedLoadZoneCapacityWhenAddingResources()
        {
            // Тест на переполнение зоны загрузки при добавлении ресурсов
        }

        [Test]
        public void ShouldPullResourcesFromLoadZone()
        {
            // Тест на успешное изъятие ресурсов из зоны загрузки
        }

        [Test]
        public void ShouldThrowExceptionWhenPullingMoreResourcesThanAvailableInLoadZone()
        {
            // Тест на ошибку при изъятии большего количества ресурсов, чем доступно
        }

        [Test]
        public void ShouldReturnCorrectLoadZoneCapacity()
        {
            // Тест на получение корректной общей вместимости зоны загрузки
        }

        [Test]
        public void ShouldReturnCorrectFreeCapacityInLoadZone()
        {
            // Тест на получение корректного свободного места в зоне загрузки
        }

        [Test]
        public void ShouldReturnCorrectResourcesInLoadZone()
        {
            // Тест на получение списка ресурсов и их количества в зоне загрузки
        }

        // Работа с ресурсами в зоне выгрузки
        [Test]
        public void ShouldPullResourcesFromUnloadZone()
        {
            // Тест на успешное изъятие ресурсов из зоны выгрузки
        }

        [Test]
        public void ShouldThrowExceptionWhenPullingMoreResourcesThanAvailableInUnloadZone()
        {
            // Тест на ошибку при изъятии большего количества ресурсов, чем доступно в зоне выгрузки
        }

        [Test]
        public void ShouldReturnCorrectUnloadZoneCapacity()
        {
            // Тест на получение корректной общей вместимости зоны выгрузки
        }

        [Test]
        public void ShouldReturnCorrectFreeCapacityInUnloadZone()
        {
            // Тест на получение корректного свободного места в зоне выгрузки
        }

        [Test]
        public void ShouldReturnCorrectResourcesInUnloadZone()
        {
            // Тест на получение списка ресурсов и их количества в зоне выгрузки
        }

        // Управление правилами конверсии
        [Test]
        public void ShouldSetConversionRule()
        {
            // Тест на успешное добавление правила конверсии
        }

        [Test]
        public void ShouldOverrideExistingConversionRuleForSameInputResource()
        {
            // Тест на замену существующего правила для одного и того же входного ресурса
        }

        [Test]
        public void ShouldThrowExceptionWhenAddingInvalidConversionRule()
        {
            // Тест на ошибку при добавлении некорректного правила
        }
    }
}