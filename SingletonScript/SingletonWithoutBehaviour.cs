using System;
using System.Reflection;

public class SingletonWithoutBehaviourException : Exception
{
    // Exception은 천천히 가자
    public SingletonWithoutBehaviourException(Exception exception)
    {
    }

    public SingletonWithoutBehaviourException(string exceptionText)
    {
    }
}

public class SingletonWithoutBehaviour<T> where T : class
{
    private static volatile T _instance;
    private static object _lock = new object();

    static SingletonWithoutBehaviour()
    {
    }

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        ConstructorInfo constructor = null;

                        try
                        {
                            constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null);
                        }
                        catch (Exception exception)
                        {
                            throw new SingletonWithoutBehaviourException(exception);
                        }

                        if (constructor == null || constructor.IsAssembly)
                        {
                            throw new SingletonWithoutBehaviourException(string.Format("A private or protected constructor is missing for '{0}'.", typeof(T).Name));
                        }

                        _instance = (T)constructor.Invoke(null);
                    }
                }
            }

            return _instance;
        }
    }
}