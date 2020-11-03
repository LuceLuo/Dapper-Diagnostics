using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
    /// <summary>
    /// 
    /// </summary>
    public static class DiagnosticListenerExtensions
    {
        /// <summary>
        /// Listener name
        /// </summary>
        public const string DAPPER_DIAGNOSTIC_LISTENER = "DapperDiagnosticListener";
        /// <summary>
        /// Listener Instance
        /// </summary>
        public static readonly DiagnosticListener Instance = new DiagnosticListener(DAPPER_DIAGNOSTIC_LISTENER);
        /// <summary>
        /// prefix
        /// </summary>
        public const string DAPPER_PREFIX = "Dapper.";

        /// <summary>
        /// 
        /// </summary>
        public const string DAPPER_BEFORE_EXECUTE = DAPPER_PREFIX + nameof(WriteExecuteBefore);
        /// <summary>
        /// 
        /// </summary>
        public const string DAPPER_AFTER_EXECUTE = DAPPER_PREFIX + nameof(WriteExecuteAfter);
        /// <summary>
        /// 
        /// </summary>
        public const string DAPPER_ERROR_EXECUTE = DAPPER_PREFIX + nameof(WriteExecuteError);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="dbConnection"></param>
        /// <param name="commandDefinition"></param>
        /// <param name="operation"></param>
        /// <param name="isAsync"></param>
        /// <returns></returns>
        public static Guid WriteExecuteBefore(this DiagnosticListener listener, IDbConnection dbConnection, CommandDefinition commandDefinition, string operation = "Execute", bool isAsync = false)
        {
            if (!listener.IsEnabled(DAPPER_BEFORE_EXECUTE)) return Guid.Empty;
            var operationId = Guid.NewGuid();
            listener.Write(DAPPER_BEFORE_EXECUTE, new ExecuteBeforeEventData(operationId, operation)
            {
                CommandDefinition = commandDefinition,
                DbConnection = dbConnection,
                IsAsync = isAsync
            });
            return operationId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="operationId"></param>
        /// <param name="dbConnection"></param>
        /// <param name="commandDefinition"></param>
        /// <param name="operation"></param>
        /// <param name="isAsync"></param>
        public static void WriteExecuteAfter(this DiagnosticListener listener, Guid operationId, IDbConnection dbConnection, CommandDefinition commandDefinition, string operation = "Execute", bool isAsync = false)
        {
            if (listener.IsEnabled(DAPPER_AFTER_EXECUTE))
            {
                listener.Write(DAPPER_AFTER_EXECUTE, new ExecuteAfterEventData(operationId, operation)
                {
                    CommandDefinition = commandDefinition,
                    DbConnection = dbConnection,
                    IsAsync = isAsync
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="operationId"></param>
        /// <param name="dbConnection"></param>
        /// <param name="commandDefinition"></param>
        /// <param name="ex"></param>
        /// <param name="operation"></param>
        /// <param name="isAsync"></param>
        public static void WriteExecuteError(this DiagnosticListener listener, Guid operationId, IDbConnection dbConnection, CommandDefinition commandDefinition, Exception ex, string operation = "Execute", bool isAsync = false)
        {
            if (listener.IsEnabled(DAPPER_ERROR_EXECUTE))
            {
                listener.Write(DAPPER_ERROR_EXECUTE, new ExecuteErrorEventData(operationId, operation)
                {
                    CommandDefinition = commandDefinition,
                    DbConnection = dbConnection,
                    IsAsync = isAsync,
                    Exception = ex
                });
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EventData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationId"></param>
        /// <param name="operation"></param>
        public EventData(Guid operationId, string operation)
        {
            OperationId = operationId;
            Operation = operation;
        }
        /// <summary>
        /// 
        /// </summary>
        public Guid OperationId { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Operation { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IErrorEventData
    {
        /// <summary>
        /// 
        /// </summary>
        Exception Exception { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExecuteEventData : EventData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationId"></param>
        /// <param name="operation"></param>
        public ExecuteEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandDefinition CommandDefinition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IDbConnection DbConnection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAsync { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExecuteBeforeEventData : ExecuteEventData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationId"></param>
        /// <param name="operation"></param>
        public ExecuteBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExecuteAfterEventData : ExecuteEventData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationId"></param>
        /// <param name="operation"></param>
        public ExecuteAfterEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExecuteErrorEventData : ExecuteEventData, IErrorEventData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationId"></param>
        /// <param name="operation"></param>
        public ExecuteErrorEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public Exception Exception { get; set; }
    }
}
