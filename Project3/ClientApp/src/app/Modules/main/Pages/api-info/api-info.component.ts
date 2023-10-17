import { Component, OnInit } from '@angular/core';
import { apiInfo } from '../../Models/apiInfo';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-api-info',
  templateUrl: './api-info.component.html',
  styleUrls: ['./api-info.component.scss'],
})
export class ApiInfoComponent {
  private deviceUrl = 'http://localhost:5000/api/DeviceConfig';
  private attendanceUrl = 'http://localhost:5000/api/AttendanceLog';
  private userUrl = 'http://localhost:5000/api/User';

  displayedColumns: string[] = ['title', 'type', 'url', 'params'];

  ApiData: apiInfo[] = [
    {
      title: 'GetDeviceConfigLIVE',
      type: 'PATCH',
      url: `${this.deviceUrl}/GetDeviceConfigCZKEM`,
      params: '',
    },
    {
      title: 'EditDeviceConfig',
      type: 'PATCH',
      url: `${this.deviceUrl}/EditDeviceConfig?id=value`,
      params:"JSON: "
    },
    {
      title: 'RemoveDeviceConfig',
      type: 'DELETE',
      url: `${this.deviceUrl}/RemoveDevice/`,
      params: '?deviceId=value',
    },
    {
      title: 'AddNewDevice',
      type: 'POST',
      url: `${this.deviceUrl}/AddNewDevice`,
      params: "JSON: {attribute1:value1,attribute2:value2,... }",
    },
    {
      title: 'GetMultipleDevicesInfoLIVE',
      type: 'POST',
      url: `${this.deviceUrl}/GetMultipleDevicesCZKEM`,
      params: '[id1, id2,...] ',
    },

    {
      title: 'GetAttendanceLog',
      type: 'GET',
      url: `${this.userUrl}/GetUserAttendanceLog`,
      params: 'For Pagination: ?pageNumber=value&pageSize=value',
    },
    {
      title: 'GetUpdatedAttendanceLog',
      type: 'GET',
      url: `${this.attendanceUrl}/GetUpdatedAttendanceLog`,
      params: '',
    },
    {
      title: 'GetUserAttendanceLogOfMultipleDevicesLIVE',
      type: 'POST',
      url: `${this.attendanceUrl}/GetUserAttendanceLogOfMultipleDevicesLIVE`,
      params: "[JSON]: [id1,id2]",
    },
    {
      title: 'FilteredAttendanceLog',
      type: 'GET',
      url: `${this.attendanceUrl}/filter`,
      params: '?attrbute1=value&attribute2=value&..',
    },
    {
      title: 'GetUserInfo',
      type: 'GET',
      url: `${this.userUrl}/GetUserInfo`,
      params: "",
    },
    {
      title: 'GetUserInfoLIVE',
      type: 'GET',
      url: `${this.userUrl}/GetUserInfoCZKEM`,
      params: '',
    },
  ];
}


