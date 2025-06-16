// [corporate-hate] An adapter pattern because we need 4 files instead of 1 to feel enterprise-y 🏢

import { ExternalLoggerService } from "./ExternalLoggerService";
import { ILogger } from "./ILogger";

export class LoggerAdapter implements ILogger {
  // [corporate-hate] Dependency injection because direct instantiation is so 2010
  private externalLogger: ExternalLoggerService;

  constructor(externalLogger: ExternalLoggerService) {
    // [corporate-hate] Constructor injection: making simple things complicated since forever
    this.externalLogger = externalLogger;
  }

  info(message: string): void {
    // [corporate-hate] Delegating to another logger because one logger is never enough
    this.externalLogger.logInfo(`💡 [info]: ${message}`);
  }

  warn(message: string): void {
    // [corporate-hate] Because warnings should be sent to 5 different monitoring systems
    this.externalLogger.logWarn(`⚠️ [warn]: ${message}`);
    this.externalLogger.sendToRemoteLogging("WARN", message);
  }

  error(message: string): void {
    // [corporate-hate] More delegation than a corporate hierarchy
    this.externalLogger.logError(`🔥 [error]: ${message}`);
    this.externalLogger.sendToRemoteLogging("ERROR", message);
  }

  debug?(message: string): void {
    // [corporate-hate] Debug logs that no one will ever check
    this.externalLogger.logDebug(`🧪 [debug]: ${message}`);
  }
}
