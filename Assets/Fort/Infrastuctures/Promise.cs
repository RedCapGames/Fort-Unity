using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Fort
{
    public interface IPromise
    {

    }

    public class Deferred<TComplition, TError>
    {
        private Promise<TComplition, TError> _promise;

        public Deferred()
        {
            _promise = new Promise<TComplition, TError>();
        }
        public Promise<TComplition, TError> Promise()
        {
            return _promise;
        }

        public void Resolve(TComplition complition)
        {
            _promise.Resolve(complition);
        }

        public void Reject(TError error)
        {
            _promise.Reject(error);
        }

    }
    internal class InternalPromise<TComplition, TError> : IPromise
    {
        private TComplition _complitionResult;
        private TError _errorResult;
        private ResultStatus _resultStatus;
        private Action<TComplition> _completion;
        private Action<TError> _error;
        private IPromiseCapture _promiseCapture;
        private int _promiseCallIndex;

        public IPromiseCapture PromiseCapture
        {
            get { return _promiseCapture; }
            set
            {
                _promiseCapture = value;
                _promiseCapture.PromiseCaptureToken.CallIndex++;
                _promiseCallIndex = _promiseCapture.PromiseCaptureToken.CallIndex;
            }
        }

        public InternalPromise()
        {
            _resultStatus = ResultStatus.ResultNotRiched;
        }

        private bool IsCallCapable()
        {
            return _promiseCapture == null ||
                   _promiseCapture.PromiseCaptureToken.CancelIndex < _promiseCallIndex;
        }

        public void Then(Action<TComplition> completion)
        {
            Then(completion, null);
        }
        public void Then(Action<TComplition> completion, Action<TError> error)
        {
            switch (_resultStatus)
            {
                case ResultStatus.Completed:
                    try
                    {
                        if (IsCallCapable())
                            completion(_complitionResult);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                    break;
                case ResultStatus.Error:
                    try
                    {
                        if (IsCallCapable())
                            error(_errorResult);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                    break;
                case ResultStatus.ResultNotRiched:
                    _completion = completion;
                    _error = error;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal void Resolve(TComplition complition)
        {
            _complitionResult = complition;
            _resultStatus = ResultStatus.Completed;
            if (_completion != null)
            {
                try
                {
                    if (IsCallCapable())
                        _completion(complition);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        internal void Reject(TError error)
        {
            _errorResult = error;
            _resultStatus = ResultStatus.Error;
            if (_error != null)
            {
                try
                {
                    if (IsCallCapable())
                        _error(error);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        private enum ResultStatus
        {
            Completed,
            Error,
            ResultNotRiched
        }
    }
    public class Promise<TComplition, TError> : IPromise
    {
        private InternalPromise<object, object> _internalPromise;

        public Promise<TComplition, TError> Capture(IPromiseCapture promiseCapture)
        {
            if (_internalPromise.PromiseCapture != null)
                throw new Exception("Double Promise capture");
            _internalPromise.PromiseCapture = promiseCapture;
            return this;
        }

        public Promise()
        {
            _internalPromise = new InternalPromise<object, object>();
        }

        public void Then(Action<TComplition> completion)
        {
            Then(completion, null);
        }
        public void Then(Action<TComplition> completion, Action<TError> error)
        {
            _internalPromise.Then(o => completion((TComplition)o), o =>
            {
                if (error != null)
                    error((TError)o);
            });
        }

        internal void Resolve(TComplition complition)
        {
            _internalPromise.Resolve(complition);
        }
        internal void Reject(TError error)
        {
            _internalPromise.Reject(error);
        }
    }

    public class Deferred
    {
        private Promise _promise;

        public Deferred()
        {
            _promise = new Promise();
        }
        public Promise Promise()
        {
            return _promise;
        }

        public void Resolve()
        {
            _promise.Resolve();
        }

        public void Reject()
        {
            _promise.Reject();
        }

    }
    public class Promise : IPromise
    {
        private InternalPromise<object, object> _internalPromise;

        public Promise()
        {
            _internalPromise = new InternalPromise<object, object>();
        }

        public Promise Capture(IPromiseCapture promiseCapture)
        {
            if (_internalPromise.PromiseCapture != null)
                throw new Exception("Double Promise capture");
            _internalPromise.PromiseCapture = promiseCapture;
            return this;
        }

        public void Then(Action completion)
        {
            Then(completion, null);
        }
        public void Then(Action completion, Action error)
        {
            _internalPromise.Then(o => completion(), o => error());
        }

        internal void Resolve()
        {
            _internalPromise.Resolve(null);
        }
        internal void Reject()
        {
            _internalPromise.Reject(null);
        }
    }

    public class ErrorDeferred<TError>
    {
        private ErrorPromise<TError> _promise;

        public ErrorDeferred()
        {
            _promise = new ErrorPromise<TError>();
        }

        public ErrorPromise<TError> Promise()
        {
            return _promise;
        }

        public void Resolve()
        {
            _promise.Resolve();
        }

        public void Reject(TError error)
        {
            _promise.Reject(error);
        }

    }
    public class ErrorPromise<TError> : IPromise
    {
        private InternalPromise<object, object> _internalPromise;
        public ErrorPromise()
        {
            _internalPromise = new InternalPromise<object, object>();
        }
        public ErrorPromise<TError> Capture(IPromiseCapture promiseCapture)
        {
            if (_internalPromise.PromiseCapture != null)
                throw new Exception("Double Promise capture");
            _internalPromise.PromiseCapture = promiseCapture;
            return this;
        }
        public void Then(Action completion)
        {
            Then(completion, null);
        }
        public void Then(Action completion, Action<TError> error)
        {
            _internalPromise.Then(o => completion(), o =>
            {
                if (error != null)
                    error((TError)o);
            });
        }

        internal void Resolve()
        {
            _internalPromise.Resolve(null);
        }
        internal void Reject(TError error)
        {
            _internalPromise.Reject(error);
        }
    }

    public class ComplitionDeferred<TComplition>
    {
        public ComplitionDeferred()
        {
            _promise = new ComplitionPromise<TComplition>();
        }
        private ComplitionPromise<TComplition> _promise;
        public ComplitionPromise<TComplition> Promise()
        {
            return _promise;
        }

        public void Resolve(TComplition complition)
        {
            _promise.Resolve(complition);
        }

        public void Reject()
        {
            _promise.Reject();
        }

    }
    public class ComplitionPromise<TComplition> : IPromise
    {
        private InternalPromise<object, object> _internalPromise;

        public ComplitionPromise()
        {
            _internalPromise = new InternalPromise<object, object>();
        }
        public ComplitionPromise<TComplition> Capture(IPromiseCapture promiseCapture)
        {
            if (_internalPromise.PromiseCapture != null)
                throw new Exception("Double Promise capture");
            _internalPromise.PromiseCapture = promiseCapture;
            return this;
        }

        public void Then(Action<TComplition> completion)
        {
            Then(completion, null);
        }
        public void Then(Action<TComplition> completion, Action error)
        {
            _internalPromise.Then(o => completion((TComplition)o), o =>
            {
                if (error != null)
                    error();
            });
        }

        internal void Resolve(TComplition complition)
        {
            _internalPromise.Resolve(complition);
        }
        internal void Reject()
        {
            _internalPromise.Reject(null);
        }
    }

    public static class PromiseExtensions
    {
        public static void Then<TComplition>(this ComplitionPromise<TComplition> promise, Action<bool, TComplition> action)
        {
            promise.Then(complition => action(true, complition), () => action(false, default(TComplition)));
        }

        public static void Then<TError>(this ErrorPromise<TError> promise, Action<bool, TError> action)
        {
            promise.Then(() => action(true, default(TError)), error => action(false, error));
        }

        public static void Then(this Promise promise, Action<bool> action)
        {
            promise.Then(() => action(true), () => action(false));
        }

        public static void Then<TComplition, TError>(this Promise<TComplition, TError> promise,
            Action<bool, TComplition, TError> action)
        {
            promise.Then(complition => action(true, complition, default(TError)), error => action(false, default(TComplition), error));
        }
    }
    public static class PromiseHelper
    {
        public static Promise<PromiseAllResult[], PromiseAllResult[]> ThenAll(IPromise[] promises)
        {
            int resolveCount = 0;
            PromiseAllResult[] promiseAllResult = new PromiseAllResult[promises.Length];

            Deferred<PromiseAllResult[], PromiseAllResult[]> result = new Deferred<PromiseAllResult[], PromiseAllResult[]>();

            Action checkToResolve = () =>
            {
                if (resolveCount != promises.Length)
                    return;
                if (promiseAllResult.All(result1 => result1.Succeded))
                    result.Resolve(promiseAllResult);
                else
                    result.Reject(promiseAllResult);
            };
            int counter = 0;
            foreach (IPromise promise in promises)
            {
                int index = counter;
                InternalPromise<object, object> innerPromise = (InternalPromise<object, object>)promise.GetType().GetField("_internalPromise", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(promise);
                innerPromise.Then(o =>
                {
                    resolveCount++;
                    promiseAllResult[index] = new PromiseAllResult
                    {
                        Result = o,
                        Succeded = true
                    };
                    checkToResolve();
                }, o =>
                {
                    resolveCount++;
                    promiseAllResult[index] = new PromiseAllResult
                    {
                        Result = o,
                        Succeded = false
                    };
                    checkToResolve();
                });
                counter++;

            }
            return result.Promise();
        }
        public static Promise<PromiseAnyResult, object[]> Any(IPromise[] promises)
        {
            int resolveCount = 0;
            object[] promiseResult = new object[promises.Length];

            Deferred<PromiseAnyResult, object[]> result = new Deferred<PromiseAnyResult, object[]>();
            int counter = 0;
            foreach (IPromise promise in promises)
            {
                int index = counter;
                Promise<object, object> innerPromise = (Promise<object, object>)promise.GetType().GetField("_internalPromise").GetValue(promise);
                innerPromise.Then(o =>
                {
                    result.Resolve(new PromiseAnyResult
                    {
                        Index = index,
                        Result = o
                    });
                }, o =>
                {
                    promiseResult[index] = o;
                    resolveCount++;
                    if (resolveCount == promises.Length)
                        result.Reject(promiseResult);

                });
                counter++;

            }
            return result.Promise();
        }
    }

    public class PromiseAllResult
    {
        public bool Succeded { get; set; }
        public object Result { get; set; }
    }
    public class PromiseAnyResult
    {
        public int Index { get; set; }
        public object Result { get; set; }
    }

    public interface IPromiseCapture
    {
        PromiseCaptureToken PromiseCaptureToken { get; }
    }
    public class PromiseCaptureToken
    {
        public PromiseCaptureToken()
        {
            CallIndex = 1;
        }
        public int CallIndex { get; set; }
        public int CancelIndex { get; set; }
        public void Purge()
        {
            CancelIndex = CallIndex;
        }
    }

}
