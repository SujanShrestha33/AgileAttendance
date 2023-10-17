import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MainRoutingModule } from './main-routing.module';
import { AttendenceLogComponent } from './Pages/attendence-log/attendence-log.component';
import { UserInfoComponent } from './Pages/user-info/user-info.component';
import { DeviceConfigComponent } from './Pages/device-config/device-config.component';
import { HttpClientModule } from '@angular/common/http';
// import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FormsModule } from '@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
// import { NavbarComponent } from './Components/navbar/navbar.component';
import {MatPaginatorModule} from '@angular/material/paginator';
import { ProgressbarModule } from 'ngx-bootstrap/progressbar';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { ProfileComponent } from './Pages/profile/profile.component';
import { MatFormField, MatFormFieldModule } from '@angular/material/form-field';
import { ApiInfoComponent } from './Pages/api-info/api-info.component';
import { NgSelectModule } from '@ng-select/ng-select';
// import { AngularFontAwesomeModule } from 'angular-font-awesome';

@NgModule({
  declarations: [
    AttendenceLogComponent,
    UserInfoComponent,
    DeviceConfigComponent,
    ProfileComponent,
    ApiInfoComponent,
    // NavbarComponent,
    // MainComponent
  ],
  imports: [
    CommonModule,
    MainRoutingModule,
    HttpClientModule,
    DatePipe,
    // FontAwesomeModule,
    // NgModule,
    FormsModule,
    BsDropdownModule,
    MatPaginatorModule,
    MatProgressBarModule,
    ProgressbarModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    NgSelectModule,
  ]
})
export class MainModule { }
