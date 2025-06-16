import { interfaces } from 'inversify';
interface ResolveDecorator {
    (serviceIdentifier: interfaces.ServiceIdentifier<unknown>): (target: any, name: string, descriptor?: any) => any;
    (target: any, name: string, descriptor?: any): any;
    optional: ResolveOptionalDecorator;
    all: ResolveAllDecorator;
}
interface ResolveOptionalDecorator {
    <T>(serviceIdentifier: interfaces.ServiceIdentifier<T>, defaultValue?: T): (target: any, name: string, descriptor?: any) => any;
    (target: any, name: string, descriptor?: any): any;
    all: ResolveAllDecorator;
}
interface ResolveAllDecorator {
    <T>(serviceIdentifier: interfaces.ServiceIdentifier<T>): (target: any, name: string, descriptor?: any) => any;
    (target: any, name: string, descriptor?: any): any;
}
declare const resolve: ResolveDecorator;
export { resolve };
export default resolve;
