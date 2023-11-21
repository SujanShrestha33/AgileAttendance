import { Component, OnInit } from '@angular/core';
import { UserServiceService } from '../../Services/user-service.service';
import { UserInfo } from '../../Models/user-info';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss']
})
export class UserInfoComponent implements OnInit {
  isLoading : boolean;
  userInfo : UserInfo[] = [];
  type : string;
  searchQuery: string = '';
  filteredUserInfo: UserInfo[] = [];
  selectedDeviceIds = [];
  pageNumber = 1;
  pageSize = 50;
  totalRecords = 0;

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
        this.selectedDeviceIds = params['selectedDeviceIds'];
        this.type = params['type'];
        // console.log(this.type);     
        // console.log(this.selectedDeviceIds);
      }
    );
    // throw new Error('Method not implemented.');
    if(this.selectedDeviceIds != undefined){
        this.getUsersInfoSelectedDeviceLive();
    }
    else if (this.type === 'live'){
      // console.log('live');
      this.getAllUsersLive();
    }
    else{
      this.getAllUsers();
    }
  }

  constructor (
    private userService : UserServiceService,
    private route : ActivatedRoute
  ) { }

  getAllUsers(){
    this.isLoading = true;
    this.userService.getUserInfo()
      .subscribe(res => {
        this.userInfo = res;
        // console.log(res);
        this.filteredUserInfo = this.userInfo;
        // console.log(this.filteredUserInfo);
        this.totalRecords = res.length;
        // console.log(this.totalRecords);
        // console.log(res)
        // console.log(this.userInfo);
        this.isLoading = false;
      }, error => {
        this.isLoading = false;
      })
  }

  getAllUsersLive(){
    this.isLoading = true;
    this.userService.getUserInfoLive()
      .subscribe(res => {
        this.userInfo = res;
        this.filteredUserInfo = this.userInfo;
        // console.log(res)
        // console.log(this.filteredUserInfo);
        this.totalRecords = res.length;
        this.isLoading = false;
      }, error => {
        this.isLoading = false;
      })
  }

  getUsersInfoSelectedDeviceLive(){
    this.isLoading = true;
    this.userService.getMultipleDeviceUserInfo(this.selectedDeviceIds)
      .subscribe(res => {
        this.userInfo = res;
        this.filteredUserInfo = this.userInfo;
        // console.log(res)
        // console.log(this.filteredUserInfo);
        this.totalRecords = res.length;
        this.isLoading = false;
      }, error => {
        this.isLoading = false;
      })
  }

  performSearch(): void {
    // Convert the search query to lowercase for case-insensitive search.
    const query = this.searchQuery.toLowerCase();

    // Filter the deviceInfo array based on the search query.
    this.filteredUserInfo = this.userInfo.filter(
      (item) =>
        item.name.toLowerCase().includes(query) ||
        // item.deviceName.toLowerCase().includes(query) ||
        item.enrollNumber.toLowerCase().includes(query) ||
        item.deviceId.toString().includes(query)
    );
    // console.log(this.filteredUserInfo);
  }

  onPageChange(event: any): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
  }

}
