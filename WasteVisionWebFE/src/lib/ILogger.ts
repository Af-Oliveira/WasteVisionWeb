// [corporate-hate] Interfaces - because every simple console.log deserves its own enterprise architecture 🤡

export interface ILogger {
  info(message: string): void;
  warn(message: string): void;
  error(message: string): void;
  debug?(message: string): void;
}
