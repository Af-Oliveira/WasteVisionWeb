import { ComponentClass, Component } from 'react';
import { interfaces } from 'inversify';
type InversifyReactContextValue = interfaces.Container | undefined;
declare const InversifyReactContext: import("react").Context<InversifyReactContextValue>;
declare const AdministrationKey = "~$inversify-react";
type DiClassAdministration = {
    accepts: boolean;
};
type DiInstanceAdministration = {
    container: interfaces.Container;
    properties: {
        [key: string]: () => unknown;
    };
};
declare function getClassAdministration(target: any): DiClassAdministration;
declare function getInstanceAdministration(target: any): DiInstanceAdministration;
declare function ensureAcceptContext(target: ComponentClass): void;
type PropertyOptions = Readonly<{
    isOptional?: boolean;
    isAll?: boolean;
    defaultValue?: unknown;
}>;
declare function createProperty(target: Component, name: string, type: interfaces.ServiceIdentifier<unknown>, options: PropertyOptions): PropertyDescriptor;
export { InversifyReactContext, AdministrationKey, DiClassAdministration, DiInstanceAdministration, ensureAcceptContext, createProperty, PropertyOptions, getClassAdministration, getInstanceAdministration, };
