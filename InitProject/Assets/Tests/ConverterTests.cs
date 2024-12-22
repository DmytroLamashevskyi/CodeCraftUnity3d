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
            // Arrange
            IConverter converter = new Converter(10, 15);
            var rule = new ConversionRule(Resource.Wood, Resource.Plank, 3, 2, 2.0f);
            converter.SetConversionRule(rule);
            converter.AddResourcesLoadZone(Resource.Wood, 6);

            bool eventTriggered = false;
            converter.ProcessingComplete += resource =>
            {
                if(resource == Resource.Plank)
                    eventTriggered = true;
            };

            // Act
            converter.Enable();
            System.Threading.Thread.Sleep(2500);

            // Assert
            Assert.IsTrue(eventTriggered, "ProcessingComplete event should be triggered when processing finishes.");
        }

        [Test]
        public void ShouldTriggerZoneOverflowEventWhenZoneIsOverflowing()
        {
            // Arrange
            IConverter converter = new Converter(5, 10);
            bool eventTriggered = false;
            string overflowedZone = null;

            converter.ZoneOverflow += zone =>
            {
                eventTriggered = true;
                overflowedZone = zone;
            };

            // Act
            converter.AddResourcesLoadZone(Resource.Wood, 6);

            // Assert
            Assert.IsTrue(eventTriggered, "ZoneOverflow event should be triggered when the load zone overflows.");
            Assert.AreEqual("Load Zone", overflowedZone, "Overflowed zone should be correctly identified as 'Load Zone'.");
        }

        // Управление состоянием
        [Test]
        public void ShouldEnableConverter()
        {
            // Arrange
            IConverter converter = new Converter(5, 10);

            // Act
            converter.Enable();

            // Assert
            Assert.IsTrue(converter.IsEnabled(), "Converter should be enabled after calling Enable.");
        }

        [Test]
        public void ShouldDisableConverter()
        {
            // Arrange
            IConverter converter = new Converter(5, 10);
            converter.Enable();

            // Act
            converter.Disable();

            // Assert
            Assert.IsFalse(converter.IsEnabled(), "Converter should be disabled after calling Disable.");
        } 

        // Управление процессом обработки
        [Test]
        public void ShouldReturnTrueWhenProcessing()
        {
            // Arrange
            IConverter converter = new Converter(5, 10);
            converter.SetConversionRule(new ConversionRule(Resource.Wood, Resource.Plank, 3, 2, 5.0f));
            converter.AddResourcesLoadZone(Resource.Wood, 3);
            converter.Enable();

            // Act
            bool isProcessing = converter.IsProcessing(); 

            // Assert
            Assert.IsTrue(isProcessing, "IsProcessing should return true when the converter is actively processing resources.");
        }

        [Test]
        public void ShouldReturnFalseWhenNotProcessing()
        {
            // Arrange
            IConverter converter = new Converter(5, 10);

            // Act
            bool isProcessing = converter.IsProcessing();

            // Assert
            Assert.IsFalse(isProcessing, "IsProcessing should return false when the converter is not processing resources.");
        }

        [Test] 
        public void ShouldReturnCorrectTimeLeftForProcessing()
        {
            // Arrange
            IConverter converter = new Converter(5, 10);
            converter.SetConversionRule(new ConversionRule(Resource.Wood, Resource.Plank, 3, 2, 10.0f));
            converter.AddResourcesLoadZone(Resource.Wood, 3);
            converter.Enable();
            System.Threading.Thread.Sleep(3000);

            // Act
            float timeLeft = converter.TimeLeft();

            // Assert
            Assert.IsTrue(timeLeft >= 6.9f && timeLeft <= 7.1f,
                $"Expected time left to be around 7 seconds, but got {timeLeft}.");
        }

        // Работа с ресурсами в зоне загрузки
        [Test]
        public void ShouldAddResourcesToLoadZone()
        {
            // Arrange
            IConverter converter = new Converter(10, 15);
            var resourceToAdd = Resource.Wood;
            var amountToAdd = 5;

            // Act
            converter.AddResourcesLoadZone(resourceToAdd, amountToAdd);
            var loadZoneResources = converter.GetLoadZoneResources();

            // Assert
            Assert.IsTrue(loadZoneResources.ContainsKey(resourceToAdd), "The resource should exist in the load zone.");
            Assert.AreEqual(amountToAdd, loadZoneResources[resourceToAdd], $"Expected {amountToAdd} {resourceToAdd} in the load zone, but got {loadZoneResources[resourceToAdd]}.");
        }

        [Test] 
        public void ShouldNotExceedLoadZoneCapacityWhenAddingResources()
        {
            // Arrange
            IConverter converter = new Converter(10, 15);
            var resourceToAdd = Resource.Wood;
            var firstBatch = 7;
            var secondBatch = 5;

            // Act
            converter.AddResourcesLoadZone(resourceToAdd, firstBatch);
            converter.AddResourcesLoadZone(resourceToAdd, secondBatch);

            var loadZoneResources = converter.GetLoadZoneResources();

            // Assert
            Assert.AreEqual(10, loadZoneResources[resourceToAdd], $"Expected load zone capacity to be capped at 10 for {resourceToAdd}, but got {loadZoneResources[resourceToAdd]}.");
        }

        [Test]
        public void ShouldPullResourcesFromLoadZone()
        {
            // Arrange
            IConverter converter = new Converter(10, 15);
            var resourceToAdd = Resource.Wood;
            var initialAmount = 8;
            var amountToPull = 5;

            converter.AddResourcesLoadZone(resourceToAdd, initialAmount);

            // Act
            converter.PullResourcesLoadZone(resourceToAdd, amountToPull);
            var loadZoneResources = converter.GetLoadZoneResources();

            // Assert
            Assert.AreEqual(initialAmount - amountToPull, loadZoneResources[resourceToAdd],
                $"Expected {initialAmount - amountToPull} {resourceToAdd} in the load zone after pulling, but got {loadZoneResources[resourceToAdd]}.");
        }

        [Test]
        public void ShouldThrowExceptionWhenPullingMoreResourcesThanAvailableInLoadZone()
        {
            // Arrange
            IConverter converter = new Converter(10, 15);
            var resourceToAdd = Resource.Wood;
            var initialAmount = 3;
            var amountToPull = 5;

            converter.AddResourcesLoadZone(resourceToAdd, initialAmount);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                converter.PullResourcesLoadZone(resourceToAdd, amountToPull);
            });

            Assert.AreEqual($"Not enough {resourceToAdd} in load zone to pull.", ex.Message,
                "The exception message should indicate that there are not enough resources to pull.");
        }


        [Test]
        public void ShouldReturnCorrectLoadZoneCapacity()
        {
            // Arrange
            int expectedCapacity = 10;
            IConverter converter = new Converter(expectedCapacity, 15);

            // Act
            int actualCapacity = converter.GetLoadZoneCapacity();

            // Assert
            Assert.AreEqual(expectedCapacity, actualCapacity,
                $"Expected load zone capacity to be {expectedCapacity}, but got {actualCapacity}.");
        }

        [Test]
        public void ShouldReturnCorrectFreeCapacityInLoadZone()
        {
            // Arrange
            int loadZoneCapacity = 10;
            IConverter converter = new Converter(loadZoneCapacity, 15);
            var resourceToAdd = Resource.Wood;
            var amountToAdd = 4;

            converter.AddResourcesLoadZone(resourceToAdd, amountToAdd);

            // Act
            int freeCapacity = converter.GetFreeLoadZoneCapacity();

            // Assert
            int expectedFreeCapacity = loadZoneCapacity - amountToAdd;
            Assert.AreEqual(expectedFreeCapacity, freeCapacity,
                $"Expected free capacity in load zone to be {expectedFreeCapacity}, but got {freeCapacity}.");
        }

        [Test]
        public void ShouldReturnCorrectResourcesInLoadZone()
        {
            // Arrange
            IConverter converter = new Converter(10, 15);
            var resourceToAdd = Resource.Wood;
            var amountToAdd = 5;

            converter.AddResourcesLoadZone(resourceToAdd, amountToAdd);

            // Act
            var loadZoneResources = converter.GetLoadZoneResources();

            // Assert
            Assert.IsTrue(loadZoneResources.ContainsKey(resourceToAdd), $"Expected {resourceToAdd} to be present in the load zone.");
            Assert.AreEqual(amountToAdd, loadZoneResources[resourceToAdd],
                $"Expected {amountToAdd} {resourceToAdd} in the load zone, but got {loadZoneResources[resourceToAdd]}.");
        }

        // Работа с ресурсами в зоне выгрузки
        [Test] 
        public void ShouldPullResourcesFromUnloadZone()
        {
            // Arrange
            IConverter converter = new Converter(10, 15);
            var rule = new ConversionRule(Resource.Wood, Resource.Plank, 3, 2, 1.0f);
            converter.SetConversionRule(rule);
            converter.AddResourcesLoadZone(Resource.Wood, 9); // Достаточно для 3 обработок

            converter.Enable();
            System.Threading.Thread.Sleep(2000); // Ждем завершения обработки

            // Act
            var unloadZoneResources = converter.GetUnloadZoneResources();
            converter.PullResourcesUnloadZone(Resource.Plank, 2); // Пытаемся извлечь 2 Plank

            // Assert
            Assert.IsTrue(unloadZoneResources.ContainsKey(Resource.Plank), "Unload zone should contain Plank.");
            Assert.AreEqual(2, unloadZoneResources[Resource.Plank], "Unload zone should have 2 Plank remaining.");
        }




        [Test]
        public void ShouldThrowExceptionWhenPullingMoreResourcesThanAvailableInUnloadZone()
        {
            // Arrange
            IConverter converter = new Converter(10, 15);
            var resourceToAdd = Resource.Plank;
            var initialAmount = 5;
            var amountToPull = 10;

            converter.AddResourcesLoadZone(Resource.Wood, 15);
            converter.SetConversionRule(new ConversionRule(Resource.Wood, resourceToAdd, 3, 2, 0.5f));
            converter.Enable();
            System.Threading.Thread.Sleep(2000);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                converter.PullResourcesUnloadZone(resourceToAdd, amountToPull);
            });

            Assert.AreEqual($"Not enough {resourceToAdd} in unload zone to pull.", ex.Message,
                "The exception message should indicate that there are not enough resources to pull.");
        }

        [Test]
        public void ShouldReturnCorrectUnloadZoneCapacity()
        {
            // Arrange
            int expectedCapacity = 15;
            IConverter converter = new Converter(10, expectedCapacity);

            // Act
            int actualCapacity = converter.GetUnloadZoneCapacity();

            // Assert
            Assert.AreEqual(expectedCapacity, actualCapacity,
                $"Expected unload zone capacity to be {expectedCapacity}, but got {actualCapacity}.");
        }

        [Test]
        public void ShouldReturnCorrectFreeCapacityInUnloadZone()
        {
            // Arrange
            int unloadZoneCapacity = 15;
            IConverter converter = new Converter(10, unloadZoneCapacity);
            var amountAdded = 6;

            converter.AddResourcesLoadZone(Resource.Wood, 15);
            converter.SetConversionRule(new ConversionRule(Resource.Wood, Resource.Plank, 3, 2, 0.5f));
            converter.Enable();
            System.Threading.Thread.Sleep(5000);

            // Act
            int freeCapacity = converter.GetFreeUnloadZoneCapacity();

            // Assert
            int expectedFreeCapacity = unloadZoneCapacity - amountAdded;
            Assert.AreEqual(expectedFreeCapacity, freeCapacity,
                $"Expected free capacity in unload zone to be {expectedFreeCapacity}, but got {freeCapacity}.");
        }

        [Test]
        public void ShouldReturnCorrectResourcesInUnloadZone()
        {
            // Arrange
            IConverter converter = new Converter(10, 15);
            var inputResource = Resource.Wood;
            var outputResource = Resource.Plank;
            var inputAmount = 15;
            var expectedOutputAmount = 6;

            converter.AddResourcesLoadZone(inputResource, inputAmount);
            converter.SetConversionRule(new ConversionRule(inputResource, outputResource, 3, 2, 0.5f));
            converter.Enable();
            System.Threading.Thread.Sleep(2000);

            // Act
            var unloadZoneResources = converter.GetUnloadZoneResources();

            // Assert
            Assert.IsTrue(unloadZoneResources.ContainsKey(outputResource), $"Expected {outputResource} to be present in the unload zone.");
            Assert.AreEqual(expectedOutputAmount, unloadZoneResources[outputResource],
                $"Expected {expectedOutputAmount} {outputResource} in the unload zone, but got {unloadZoneResources[outputResource]}.");
        }


        [Test]
        public void ShouldOverrideExistingConversionRuleForSameInputResource()
        {
            // Arrange
            IConverter converter = new Converter(10, 15);
            var inputResource = Resource.Wood;
            var initialRule = new ConversionRule(inputResource, Resource.Plank, 3, 2, 5.0f);
            var overriddenRule = new ConversionRule(inputResource, Resource.Steel, 4, 1, 7.0f);

            // Act
            converter.SetConversionRule(initialRule);
            converter.SetConversionRule(overriddenRule);

            // Assert
            Assert.DoesNotThrow(() => converter.SetConversionRule(overriddenRule),
                "Setting a new conversion rule for the same input resource should not throw an exception.");
        }


        [Test]
        public void ShouldThrowExceptionWhenAddingInvalidConversionRule()
        {
            // Arrange
            IConverter converter = new Converter(10, 15);

            // Примеры некорректных правил
            var invalidRule1 = new ConversionRule(Resource.Wood, Resource.Wood, 3, 2, 5.0f); 
            var invalidRule2 = new ConversionRule(Resource.Wood, Resource.Plank, 0, 2, 5.0f);
            var invalidRule3 = new ConversionRule(Resource.Wood, Resource.Plank, 3, 0, 5.0f);
            var invalidRule4 = new ConversionRule(Resource.Wood, Resource.Plank, 3, 2, -1.0f);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => converter.SetConversionRule(invalidRule1),
                "Setting a rule with identical input and output resources should throw an exception.");
            Assert.Throws<ArgumentException>(() => converter.SetConversionRule(invalidRule2),
                "Setting a rule with zero or negative input amount should throw an exception.");
            Assert.Throws<ArgumentException>(() => converter.SetConversionRule(invalidRule3),
                "Setting a rule with zero or negative output amount should throw an exception.");
            Assert.Throws<ArgumentException>(() => converter.SetConversionRule(invalidRule4),
                "Setting a rule with negative duration should throw an exception.");
        }

    }
}