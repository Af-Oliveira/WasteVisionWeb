// [corporate-hate] What's better than one logging system? TWO logging systems! 💰💰

export class ExternalLoggerService {
  // [corporate-hate] Fancy external logging service that probably costs $50k per year

  logInfo(message: string): void {
    console.info(`[EXTERN-INFO] ${message}`);
  }

  logWarn(message: string): void {
    console.warn(`[EXTERN-WARN] ${message}`);
  }

  logError(message: string): void {
    console.error(`[EXTERN-ERROR] ${message}`);
  }

  logDebug(message: string): void {
    console.debug(`[EXTERN-DEBUG] ${message}`);
  }

  async sendToRemoteLogging(level: string, message: string): Promise<void> {
    // [corporate-hate] Let's pretend we're sending this to some overpriced SaaS that your CTO read about in a LinkedIn post
    await new Promise((resolve) => setTimeout(resolve, 1000));
    console.log(
      `[EXTERN-REMOTE] Sent "${message}" to some cloud service that will be discontinued in 6 months`
    );
  }
}
