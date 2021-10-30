import { InputDeviceSource } from '../../common/input/InputDeviceSource';
import { InputDeviceTarget } from '../../common/input/InputDeviceTarget';
export declare type InputDeviceInfo = {
    id: string;
    name: string;
    deviceApi: string;
    sources: InputDeviceSource[];
    targets: InputDeviceTarget[];
};
