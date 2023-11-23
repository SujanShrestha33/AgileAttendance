import { Component, OnInit, TemplateRef } from '@angular/core';
import { DeviceConfig } from '../../Models/device-config';
import { DeviceConfigService } from '../../Services/device-config.service';
import { HttpClient } from '@angular/common/http';
// import {
//   faTrash,
//   faEdit,
//   faCheck,
//   faCancel,
//   faPlus,
//   faX,
//   faRefresh,
//   faSpinner
// } from '@fortawesome/free-solid-svg-icons';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { AddDevice } from '../../Models/add-device';
import { ToastrService } from 'ngx-toastr';
import { AttendanceService } from '../../Services/attendance.service';
import { Router } from '@angular/router';
import { getBaseUrl } from 'src/main';

@Component({
  selector: 'app-device-config',
  templateUrl: './device-config.component.html',
  styleUrls: ['./device-config.component.scss'],
})
export class DeviceConfigComponent implements OnInit {
  loading: boolean;
  // isActive: any;
  // coffee = faTrash;
  // faedit = faEdit;
  // faCheck = faCheck;
  // faCancel = faCancel;
  // faPlus = faPlus;
  // faMultiply = faX;
  // faRecycle = faRefresh;
  // faSpinner = faSpinner;
  deviceInfo: DeviceConfig[] = [];
  // isEditable : boolean = false;
  modalRef?: BsModalRef;
  config = {
    keyboard: true,
  };
  searchQuery: string = '';
  filteredDeviceInfo: DeviceConfig[] = [];
  editedDeviceConfig: DeviceConfig[] = [];
  isAdding: boolean = false;
  deviceToAdd: AddDevice = {
    name: '',
    ipaddress: '',
    deviceId: null,
    port: null,
  };

  a = getBaseUrl();
  private attUrl = `${this.a}attendancelog`;

  isLoading = false;
  smallSpinner: boolean = false;
  isRunning: boolean = false;

  constructor(
    private deviceConfigService: DeviceConfigService,
    private http: HttpClient,
    private modalService: BsModalService,
    private toastr: ToastrService,
    private attendanceService: AttendanceService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.getDeviceConfig();
  }

  getDeviceConfig() {
    this.loading = true;
    this.isLoading = true;
   
    // console.log('hello');
    this.deviceConfigService.getDeviceConfigs().subscribe(
      (res: DeviceConfig[]) => {
        this.deviceInfo = res;
        this.filteredDeviceInfo = this.deviceInfo;
        this.loading = false;
        this.isLoading = false;
        // console.log(this.deviceInfo);
      },
      (error) => {
        // console.log(error.error);
        this.loading = false;
        
      }
    );
  }

  performSearch(): void {
    // Convert the search query to lowercase for case-insensitive search.
    const query = this.searchQuery.toLowerCase();

    // Filter the deviceInfo array based on the search query.
    this.filteredDeviceInfo = this.deviceInfo.filter(
      (item) =>
        item.name.toLowerCase().includes(query) ||
        item.ipaddress.toLowerCase().includes(query) ||
        item.port.toString().includes(query) ||
        item.deviceId.toString().includes(query) ||
        (item.isActive ? 'online' : 'offline').includes(query) ||
        (item.lastSyncDate ? item.lastSyncDate.includes(query) : false)
    );
    // console.log(this.filteredDeviceInfo);
  }

  edit(item) {
    item.isEditable = true;
  }

  cancel(item) {
    item.isEditable = false;
  }

  toggleAdd() {
    this.isAdding = !this.isAdding;
  }

  clearForm() {
    this.deviceToAdd = {
      name: '',
      ipaddress: '',
      deviceId: null,
      port: null,
    };
  }

  addDevice() {
    // console.log(this.deviceToAdd);
    if (
      this.deviceToAdd.ipaddress === '' ||
      this.deviceToAdd.name === '' ||
      this.deviceToAdd.port === null ||
      this.deviceToAdd.deviceId === null
    ) {
      // console.log('cancel');
      this.toastr.error('Please fill in all required fields.');
    } else {
      this.deviceConfigService.addDevice(this.deviceToAdd).subscribe(
        (res) => {
          // console.log(res);
          this.getDeviceConfig();
          this.toggleAdd();
          this.clearForm();
          this.toastr.success('Device added successfully');
        },
        (error) => {
          this.toastr.error('Device ID already exists');
        }
      );
    }
  }

  submit(item) {
    var id = item.deviceId;
    var body = [
      {
        op: 'replace',
        path: '/name',
        value: item.name,
      },
      {
        op: 'replace',
        path: '/ipaddress',
        value: item.ipaddress,
      },
      {
        op: 'replace',
        path: '/port',
        value: item.port,
      },
      {
        op: 'replace',
        path: '/deviceId',
        value: item.deviceId,
      },
    ];

    // console.log(body);

    this.deviceConfigService.editDevice(id, body).subscribe(
      (res) => {
        this.editedDeviceConfig = res;
        // console.log(this.editedDeviceConfig);
        // console.log(this.isEditable);
        item.isEditable = false;
      },
      (error) => {
        console.log(error.error);
      }
    );
  }

  delete(item) {
    // console.log(item);
    var id = item.deviceId;
    this.deviceConfigService.deleteDevice(id).subscribe((res) => {
      // console.log('success');
      this.getDeviceConfig();
      this.toastr.warning('Device Deleted Successfully');
    });
  }

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template, this.config);
  }

  fetchAllDevice() {
    this.smallSpinner = true;
    this.deviceConfigService.fetchAllDevice().subscribe(
      (res) => {
        this.smallSpinner = false;
        // console.log(res);
        this.getDeviceConfig();
      },
      (error) => {
        this.smallSpinner = false;
      }
    );
  }

  anyDeviceSelected(): boolean {
    return this.deviceInfo.some((item) => item.isSelected);
  }

  fetchSelectedLiveStatus() {
    this.smallSpinner = true;

    const selectedDevices = this.deviceInfo.filter((item) => item.isSelected);

    if (selectedDevices.length === 0) {
      this.toastr.warning('Select at least one device to fetch live status.');
      return;
    }

    const selectedDeviceIds = selectedDevices.map((item) => item.deviceId);

    this.deviceConfigService.fetchMultipleDevice(selectedDeviceIds).subscribe(
      (liveStatusData) => {
        // console.log(selectedDeviceIds);
        // Handle the live status data, update the UI, or perform any necessary actions.
        this.getDeviceConfig();
        // console.log('Live Status Data:', liveStatusData);
        this.toastr.success('Live status fetched successfully.');
        this.smallSpinner = false;
      },
      (error) => {
        console.error('Error fetching live status:', error);
        this.toastr.error('Error fetching live status.');
        this.smallSpinner = false;
      }
    );
  }

  fetchAllAttendance() {
    const param = 'live';
    this.router.navigate(['main/logs'], { queryParams: { param } });
  }

  fetchSelectedAttendence() {

    const selectedDevices = this.deviceInfo.filter((item) => item.isSelected);
    if (selectedDevices.length === 0) {
      this.toastr.warning('Select at least one device to fetch live status.');
      return;
    }
    const selectedDeviceIds = selectedDevices.map((item) => item.deviceId);
    this.router.navigate(['main/logs'], { queryParams: { selectedDeviceIds } });
  
  }

  navigateUser() {
    const type = 'live';
    this.router.navigate(['main/users'], { queryParams: { type } });
  }

  fetchSelectedDeviceUsersInfo() {
    const selectedDevices = this.deviceInfo.filter((item) => item.isSelected);
    if (selectedDevices.length === 0) {
      this.toastr.warning('Select at least one device to fetch live status.');
      return;
    }
    const selectedDeviceIds = selectedDevices.map((item) => item.deviceId);
    this.router.navigate(['main/users'], {
      queryParams: { selectedDeviceIds },
    });
  }

 
}
