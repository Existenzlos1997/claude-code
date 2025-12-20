using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EarthUnderFreelancer.Testing
{
    /// <summary>
    /// Simple unit testing framework for game systems
    /// Run tests in Unity Editor or during development builds
    /// </summary>
    public class TestRunner : MonoBehaviour
    {
        public static TestRunner Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool runOnStart = false;
        [SerializeField] private bool logDetailedResults = true;

        private List<TestSuite> testSuites = new List<TestSuite>();
        private TestResults overallResults = new TestResults();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (runOnStart)
            {
                RunAllTests();
            }
        }

        public void RegisterTestSuite(TestSuite suite)
        {
            if (!testSuites.Contains(suite))
            {
                testSuites.Add(suite);
            }
        }

        public void RunAllTests()
        {
            UnityEngine.Debug.Log("=== Starting Test Run ===");
            overallResults = new TestResults();
            Stopwatch sw = Stopwatch.StartNew();

            foreach (var suite in testSuites)
            {
                RunTestSuite(suite);
            }

            sw.Stop();
            overallResults.totalTimeMs = (float)sw.Elapsed.TotalMilliseconds;

            PrintResults();
        }

        private void RunTestSuite(TestSuite suite)
        {
            UnityEngine.Debug.Log($"\n--- Running: {suite.Name} ---");
            
            foreach (var test in suite.Tests)
            {
                RunTest(test);
            }
        }

        private void RunTest(TestCase test)
        {
            overallResults.totalTests++;

            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                test.TestMethod();
                sw.Stop();

                overallResults.passedTests++;
                
                if (logDetailedResults)
                {
                    UnityEngine.Debug.Log($"✓ {test.Name} ({sw.ElapsedMilliseconds}ms)");
                }
            }
            catch (AssertionException ex)
            {
                overallResults.failedTests++;
                UnityEngine.Debug.LogError($"✗ {test.Name}\n  Assertion Failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                overallResults.failedTests++;
                UnityEngine.Debug.LogError($"✗ {test.Name}\n  Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void PrintResults()
        {
            UnityEngine.Debug.Log("\n" + overallResults.GetSummary());
        }

        public TestResults GetResults() => overallResults;
    }

    public class TestSuite
    {
        public string Name { get; set; }
        public List<TestCase> Tests { get; private set; } = new List<TestCase>();

        public TestSuite(string name)
        {
            Name = name;
        }

        public void AddTest(string testName, Action testMethod)
        {
            Tests.Add(new TestCase(testName, testMethod));
        }
    }

    public class TestCase
    {
        public string Name { get; set; }
        public Action TestMethod { get; set; }

        public TestCase(string name, Action testMethod)
        {
            Name = name;
            TestMethod = testMethod;
        }
    }

    public class TestResults
    {
        public int totalTests;
        public int passedTests;
        public int failedTests;
        public float totalTimeMs;

        public bool AllPassed => failedTests == 0 && totalTests > 0;
        public float PassRate => totalTests > 0 ? (float)passedTests / totalTests * 100f : 0f;

        public string GetSummary()
        {
            string status = AllPassed ? "✓ ALL TESTS PASSED" : "✗ SOME TESTS FAILED";
            
            return $@"
=== Test Results ===
{status}
Total: {totalTests}
Passed: {passedTests} ({PassRate:F1}%)
Failed: {failedTests}
Time: {totalTimeMs:F2}ms
==================";
        }
    }

    /// <summary>
    /// Assertion helper for unit tests
    /// </summary>
    public static class Assert
    {
        public static void IsTrue(bool condition, string message = "Assertion failed")
        {
            if (!condition)
            {
                throw new AssertionException($"{message}: Expected true, got false");
            }
        }

        public static void IsFalse(bool condition, string message = "Assertion failed")
        {
            if (condition)
            {
                throw new AssertionException($"{message}: Expected false, got true");
            }
        }

        public static void AreEqual<T>(T expected, T actual, string message = "Assertion failed")
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new AssertionException($"{message}: Expected {expected}, got {actual}");
            }
        }

        public static void AreNotEqual<T>(T expected, T actual, string message = "Assertion failed")
        {
            if (EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new AssertionException($"{message}: Values should not be equal: {expected}");
            }
        }

        public static void IsNull(object obj, string message = "Assertion failed")
        {
            if (obj != null)
            {
                throw new AssertionException($"{message}: Expected null, got {obj}");
            }
        }

        public static void IsNotNull(object obj, string message = "Assertion failed")
        {
            if (obj == null)
            {
                throw new AssertionException($"{message}: Expected non-null value");
            }
        }

        public static void IsGreaterThan(float value, float threshold, string message = "Assertion failed")
        {
            if (value <= threshold)
            {
                throw new AssertionException($"{message}: Expected {value} > {threshold}");
            }
        }

        public static void IsLessThan(float value, float threshold, string message = "Assertion failed")
        {
            if (value >= threshold)
            {
                throw new AssertionException($"{message}: Expected {value} < {threshold}");
            }
        }

        public static void AreApproximatelyEqual(float expected, float actual, float tolerance = 0.0001f, string message = "Assertion failed")
        {
            if (Mathf.Abs(expected - actual) > tolerance)
            {
                throw new AssertionException($"{message}: Expected ~{expected}, got {actual} (tolerance: {tolerance})");
            }
        }

        public static void Throws<T>(Action action, string message = "Assertion failed") where T : Exception
        {
            try
            {
                action();
                throw new AssertionException($"{message}: Expected {typeof(T).Name} to be thrown, but no exception was thrown");
            }
            catch (T)
            {
                // Expected exception was thrown, test passes
            }
            catch (Exception ex)
            {
                throw new AssertionException($"{message}: Expected {typeof(T).Name}, but got {ex.GetType().Name}: {ex.Message}");
            }
        }
    }

    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message) { }
    }
}
