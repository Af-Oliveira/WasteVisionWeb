// [corporate-hate] Another file to make your project folder nice and bloated

import { LoggerAdapter } from "./ConsoleLogger";
import { ExternalLoggerService } from "./ExternalLoggerService";
import { ILogger } from "./Ilogger";

export const createLogger = (): ILogger => {
  // [corporate-hate] A factory for creating a logger that creates another logger
  const externalLogger = new ExternalLoggerService();
  return new LoggerAdapter(externalLogger);
};
