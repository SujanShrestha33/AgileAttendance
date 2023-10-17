import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DeviceConfig } from '../Models/device-config';
import { map, retry } from 'rxjs';
import { AddDevice } from '../Models/add-device';
import { getBaseUrl } from 'src/main';

@Injectable({
  providedIn: 'root'
})
export class DeviceConfigService {
  a = getBaseUrl();
  baseUrl = `${this.a}deviceconfig`;

  constructor(private http : HttpClient) { }

  getDeviceConfigs(){
   return this.http.get<DeviceConfig[]>(`${this.baseUrl}/GetDeviceConfig`);
  }

  editDevice(id : number, body : any ){
    console.log(id);
    return this.http.patch<DeviceConfig[]>(`${this.baseUrl}/EditDeviceConfig?id=${id}`, body);
  }

  deleteDevice(id : number){
    console.log(id);
    return this.http.delete<any>(`${this.baseUrl}/RemoveDevice/?id=${id}`)
  }

  addDevice(body : AddDevice){
    return this.http.post<any>(`${this.baseUrl}/AddNewDevice`, body)
  }

  fetchAllDevice(){
    return this.http.patch<any>(`${this.baseUrl}/GetDeviceConfigCZKEM`, null)
  }

  fetchMultipleDevice(body : any){
    console.log(body)
    return this.http.post<any>(`${this.baseUrl}/GetMultipleDevicesCZKEM`, body)

  }

}
