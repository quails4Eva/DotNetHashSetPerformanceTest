﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DotNetHashSetPerformanceTest {

    class Program {

        private const string filePath = "C:\\Temp\\HashSetVsArrayPerformanceOutput.txt";

        private enum TestEnum {
            Test0 = 0,
            Test1,
            Test2,
            Test3,
            Test4,
            Test5,
            Test6,
            Test7,
            Test8,
            Test9,

            Test10,
            Test11,
            Test12,
            Test13,
            Test14,
            Test15,
            Test16,
            Test17,
            Test18,
            Test19,
        }

        static void Main(string[] args) {

            const int numberOfIterations = 2000000;
            const int numberOfRepeats = 40;

            for (int numElements = 0; numElements < 10; numElements++) {

                Program.WriteLine($"Elements: {numElements}, int, in range, result: {ProfileTime<int>(numberOfIterations, numberOfRepeats, numElements, true)}");
                Program.WriteLine($"Elements: {numElements}, int, out of range, result: {ProfileTime<int>(numberOfIterations, numberOfRepeats, numElements, false)}");

                Program.WriteLine($"Elements: {numElements}, string, in range, result: {ProfileTime<string>(numberOfIterations, numberOfRepeats, numElements, true)}");
                Program.WriteLine($"Elements: {numElements}, string, out of range, result: {ProfileTime<string>(numberOfIterations, numberOfRepeats, numElements, false)}");

                Program.WriteLine($"Elements: {numElements}, enum, in range, result: {ProfileTime<TestEnum>(numberOfIterations, numberOfRepeats, numElements, true)}");
                Program.WriteLine($"Elements: {numElements}, enum, out of range, result: {ProfileTime<TestEnum>(numberOfIterations, numberOfRepeats, numElements, false)}");
                Console.WriteLine();
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void WriteLine(string val) {

            Console.WriteLine(val);
            File.AppendAllText(filePath, val + Environment.NewLine);
        }

        private static ProfileResult ProfileTime<T>(int numberOfIterations, int numberOfRepeats, int numElements, bool toCheckIsInRange) {

            var numberGenerator = new Random();

            T[] arrayData = GenerateArray<T>(numElements); // Todo - maybe pass in array of data so it can be of different types
            HashSet<T> hashSetData = new HashSet<T>(arrayData);

            var stopwatchArray = new Stopwatch();
            var stopwatchHashSet = new Stopwatch();

            var results = new ProfileResult[numberOfRepeats];

            for (int j = 0; j < numberOfRepeats; j++) {

                var toCheck = GenerateValue<T>(numberGenerator, numElements, toCheckIsInRange);

                stopwatchArray.Start();

                for (int i = 0; i < numberOfIterations; i++) {

                    var contains = arrayData.Contains(toCheck);
                }

                stopwatchArray.Stop();

                var arrayTime = stopwatchArray.ElapsedMilliseconds;
                stopwatchArray.Reset();


                stopwatchHashSet.Start();

                for (int i = 0; i < numberOfIterations; i++) {

                    var contains = hashSetData.Contains(toCheck);
                }

                stopwatchHashSet.Stop();

                var hashSetTime = stopwatchHashSet.ElapsedMilliseconds;
                stopwatchHashSet.Reset();

                results[j] = new ProfileResult(arrayTime, hashSetTime);
            }

            // We skip ~ 10% of repeats to reduce the impact of CPU spikes.
            var arrayAverage = results.OrderByDescending(r => r.ArrayTime).Skip(numberOfRepeats / 10).Average(r => r.ArrayTime);
            var hashSetAverage = results.OrderByDescending(r => r.HashSetTime).Skip(numberOfRepeats / 10).Average(r => r.HashSetTime);

            return new ProfileResult(arrayAverage, hashSetAverage);
        }

        private static T[] GenerateArray<T>(int numElements) {

            var ints = Enumerable.Range(0, numElements).Cast<object>();

            if (typeof(T) == typeof(int) || typeof(T) == typeof(TestEnum)) {
                
            }
            else if (typeof(T) == typeof(string)) {

                ints = ints.Select(i => i.ToString());
            }
            else {
                throw new NotImplementedException($"Method not implemented for type {typeof(T).Name}");
            }

            return ints.Select(val => (T)val).ToArray();
        }

        private static T GenerateValue<T>(Random numberGenerator, int numElements, bool toCheckIsInRange) {

            // We use object because we may convert it to another type after int (could use a different var)
            object toCheck = numberGenerator.Next(numElements) + (toCheckIsInRange ? 0 : (numElements == 0 ? 1 : numElements));

            if (typeof(T) == typeof(int) || typeof(T) == typeof(TestEnum)) {

            }
            else if (typeof(T) == typeof(string)) {

                toCheck = toCheck.ToString();
            }
            else {
                throw new NotImplementedException($"Method not implemented for type {typeof(T).Name}");
            }

            return (T)toCheck;
        }

        private class ProfileResult {

            public decimal ArrayTime { get; set; }
            public decimal HashSetTime { get; set; }

            public ProfileResult(decimal arrayTime, decimal hashSetTime) {

                this.ArrayTime = arrayTime;
                this.HashSetTime = hashSetTime;
            }

            public override string ToString() {

                return $"Array time: {this.ArrayTime}, HashSet time: {this.HashSetTime}";
            }
        }
    }
}
