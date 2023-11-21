import { DatePipe } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { getBaseUrl } from 'src/main';

@Injectable({
  providedIn: 'root'
})
export class AttendanceService {
  a = getBaseUrl();
  private userUrl = `${this.a}user`;
  private attUrl = `${this.a}attendancelog`;

  constructor(private http : HttpClient, private datePipe: DatePipe, private toastr: ToastrService) { }

  getAttendanceLogs(pageNumber: number, pageSize: number): Observable<any> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    // console.log(params);
    return this.http.get(`${this.userUrl}/GetUserAttendanceLog`, { params });
  }

  getAllLiveAttendance(){
    return this.http.get<any>(`${this.attUrl}/GetUpdatedAttendanceLog`)
  }

  // getMultipleDeviceLiveAttendance(body: any, pageNumber: number, pageSize: number): Observable<any> {
  //   // console.log(body);
  //   const params = new HttpParams()
  //     .set('pageNumber', pageNumber.toString())
  //     .set('pageSize', pageSize.toString());
  //     // console.log(params);
  //   return this.http.post<any>(
  //     `${this.attUrl}/GetUserAttendanceLogOfMultipleDevicesLIVE`,
  //     body, // Send the request body here
  //     { params } // Send query parameters here
  //   );
  // }

  getMultipleDeviceLiveAttendance(deviceIds: any): Observable<any> {

    let params = new HttpParams()
    .set('deviceIds', deviceIds.toString());
    
    return this.http.get<any>(
      
      `${this.attUrl}/GetUserAttendanceLogOfMultipleDevicesLIVE`,
      {params} // Send the request body here
       // Send query parameters here
    );
  }

  filter(pageNumber: number, pageSize: number, deviceId:number, enrollNumber:string,deviceName:string,startDate: Date, endDate:Date, inOutMode:string, isActive:string)
  : Observable<any> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString()
      );
      if(deviceId){
        params = params.set('deviceId', deviceId.toString());
      }
      if(enrollNumber){
        params = params.set('enrollNumber', enrollNumber.toString());
      }
      // if(userName){
      //   params = params.set('userName', userName.toString());
      // }
      if(deviceName){
        params = params.set('deviceName', deviceName.toString());
      }
      
      if(startDate){
        const startInputDate = this.datePipe.transform(startDate, 'yyyy-MM-dd');
        params = params.set('startDate', startInputDate);
      }
      if(endDate ){
        const endInputDate = this.datePipe.transform(endDate, 'yyyy-MM-dd');
        params = params.set('endDate', endInputDate);
      }

      if(inOutMode){
        params = params.set('inOutMode', deviceName.toString());
      }
      if(isActive){
        params = params.set('isActive', isActive.toString());
        // console.log(params);
      }

    return this.http.get(`${this.attUrl}/filter`, { params });
  }

}
