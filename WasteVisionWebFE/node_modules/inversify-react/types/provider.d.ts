import * as React from 'react';
import { interfaces } from 'inversify';
type ProviderProps = Readonly<{
    container: interfaces.Container | (() => interfaces.Container);
    standalone?: boolean;
    children?: React.ReactNode;
}>;
declare const Provider: React.FC<ProviderProps>;
export { ProviderProps, Provider };
export default Provider;
