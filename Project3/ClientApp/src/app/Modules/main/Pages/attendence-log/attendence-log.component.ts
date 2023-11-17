import { Component, OnInit } from '@angular/core';
import { AttendanceService } from '../../Services/attendance.service';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-attendence-log',
  templateUrl: './attendence-log.component.html',
  styleUrls: ['./attendence-log.component.scss'],
})

export class AttendenceLogComponent implements OnInit {
  attendanceLogs: any[] = [];
  filteredData : any[] = [];
  //filter attributes
  deviceId: any;
  enrollNumber:any;
  userName : any;
  deviceName:any;
  startDate : any;
  endDate : any;
  inOutMode: any;
  isActive:any;

  items = [
    {booleanValue: "true", name: "Active",},
    {booleanValue: "false", name: "Inactive",}
  ]

  pageNumber = 1;
  pageSize = 50;
  totalRecords = 0;
  selectedDeviceIds = [];
  searchQuery: any;
  filteredLog = [];
  type: string;
  isLoading = false; // Add this variable
  filterBool = false; //for paginated data
  paginatedBool = false;
  testIds = [];
  responseCount:any;

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.testIds = params['selectedDeviceIds'];
      if(this.testIds != undefined){
        if(this.testIds.length == 1){
          this.selectedDeviceIds = [...params['selectedDeviceIds']] ;
        }else{
          this.selectedDeviceIds = params['selectedDeviceIds'];
        }
      }
      this.type = params['param'];
      // console.log(this.selectedDeviceIds);
      // console.log(this.testIds); // This will log an array of selected device IDs
    }
    );

    if (this.testIds != undefined) {
      this.loadMultipleDeviceLog();
    }
     else if (this.type === 'live') {
      // console.log('live');
      // this.showProgressBar = true; // Show the progress bar when making the API call
      this.loadLiveDevice();
    } else {
      this.loadAttendanceLogs();
    }
  }

  constructor(
      private attendanceService: AttendanceService,
      private route: ActivatedRoute,
      private toastr:ToastrService) {

      }

  loadLiveDevice() {
    // console.log("getting Live");
    this.isLoading = true;
    this.attendanceService.getAllLiveAttendance().subscribe(
      (res) => {
        console.log(res);
        this.loadAttendanceLogs();
        this.isLoading = false;
        // this.showProgressBar = false; // Hide the progress bar when API call is completed
      },
      (error) => {
        this.isLoading = false;
      }
    );
  }

  performSearch(): void {
    this.isLoading = true;

    // Convert the search query to lowercase for case-insensitive search.
    const query = this.searchQuery.toLowerCase();

    // Filter the deviceInfo array based on the search query.
    this.filteredLog = this.attendanceLogs.filter(
      (item) =>
        item.userName.toLowerCase().includes(query) ||
        item.deviceName.toLowerCase().includes(query) ||
        item.inputDate.toString().includes(query) ||
        item.enrollNumber.toString().includes(query) ||
        item.deviceId.toString().includes(query)
    );
    // this.attendanceService.search();
    this.isLoading = false;
    // console.log(this.filteredLog);
  }

  loadAttendanceLogs(): void {
    this.isLoading = true;
    this.attendanceService
      .getAttendanceLogs(this.pageNumber, this.pageSize)
      .subscribe(
        (response: any) => {
          this.attendanceLogs = response.data;
          this.filteredLog = this.attendanceLogs;
          console.log(this.attendanceLogs);
          this.totalRecords = response.totalRecords;
          this.isLoading = false;
        },
        (error) => {
          this.isLoading = false;
        }
      );
  }

  loadMultipleDeviceLog() {
    this.isLoading = true;
    this.paginatedBool = true;
    this.attendanceService
      .getMultipleDeviceLiveAttendance(this.selectedDeviceIds)
      .subscribe((res) => {
        // console.log('Multiple');
        // console.log(res);
        this.attendanceLogs = res.data;
        console.log(res);
        this.filteredLog = this.attendanceLogs;
        console.log(this.filteredLog);
        this.responseCount = res.totalRecords;
        console.log(this.responseCount);
        this.isLoading = false;
        // if(this.totalRecords == 0){
        //   this.toastr.info('The device is Inactive');
        // }

      }, error => {
        console.log(error.error);
        this.isLoading = false;
      });
  }

  onPageSizeChange(): void {
    this.pageNumber = 1; // Reset to the first page when the page size changes.
    this.loadAttendanceLogs();
   
  }

  onPageChange(event: any): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    
    if (!this.filterBool) {
      if (this.testIds != undefined) {
        if (this.testIds.length == 1) {
          this.loadMultipleDeviceLog();
        }
      } else {
        this.loadAttendanceLogs();
      }
    } else {
      this.filterTable();
    } 
  }

  filterTable(){
    this.isLoading = true;
    this.filterBool = true;
    this.paginatedBool = false;

    if(
        (this.deviceId == '' || this.deviceId == undefined) &&
        (this.deviceName == '' || this.deviceName == undefined ) &&
        (this.enrollNumber == '' || this.enrollNumber == undefined ) &&
        (this.userName == '' || this.userName == undefined ) &&
        (this.startDate == '' || this.startDate == undefined ) &&
        (this.endDate == '' || this.deviceId == undefined ) &&
        (this.isActive == '' || this.isActive == undefined ) 
      ){       
        this.loadAttendanceLogs();     
    }
    else{
      if(this.startDate > this.endDate){
        this.toastr.warning('Please choose start date smaller than end date');
      }
      else if(this.endDate < this.startDate){
        this.toastr.warning('Please choose end date greater than start date');
      }
      this.attendanceService
      .filter(this.pageNumber, this.pageSize, this.deviceId,this.enrollNumber,this.userName,
        this.deviceName,this.startDate,this.endDate,this.inOutMode,this.isActive)
        .subscribe((response: any) => {
          this.filteredLog = response.data;
          console.log(this.filteredLog);
          this.totalRecords = response.totalRecords;
          // console.log(this.totalRecords);
          this.isLoading = false;
        },
        error=>{
          console.log(error);
        });
      }
    }
}
