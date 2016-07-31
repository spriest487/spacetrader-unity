using UnityEngine;
using System;

public abstract class LoadValueOperation<TValueResult> : CustomYieldInstruction
    where TValueResult : struct
{
    private TValueResult? result;
    private Exception error;

    public TValueResult? Result
    {
        get
        {
            return result;
        }
        protected set
        {
            result = value;
            error = null;
        }
    }

    public Exception Error
    {
        get
        {
            return error;
        }
        protected set
        {
            result = null;
            error = value;
        }
    }

    public override bool keepWaiting
    {
        get
        {
            return !Result.HasValue && Error == null;
        }
    }
}

public abstract class LoadOperation<TResult> : CustomYieldInstruction
    where TResult : class
{
    private TResult result;
    private Exception error;

    public TResult Result
    {
        get
        {
            return result;
        }
        protected set
        {
            result = value;
            error = null;
        }
    }

    public Exception Error
    {
        get
        {
            return error;
        }
        protected set
        {
            result = null;
            error = value;
        }
    }

    public override bool keepWaiting
    {
        get
        {
            return Result == null && Error == null;
        }
    }
}
