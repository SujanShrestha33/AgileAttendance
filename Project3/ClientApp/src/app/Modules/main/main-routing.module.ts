import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AttendenceLogComponent } from './Pages/attendence-log/attendence-log.component';
import { DeviceConfigComponent } from './Pages/device-config/device-config.component';
import { UserInfoComponent } from './Pages/user-info/user-info.component';
import { ProfileComponent } from './Pages/profile/profile.component';
import { ApiInfoComponent } from './Pages/api-info/api-info.component';
// import { authGuard } from '../auth/auth.guard';

const routes: Routes = [
  {path: 'logs', component: AttendenceLogComponent},
  {path: 'devices', component: DeviceConfigComponent},
  {path: 'users', component: UserInfoComponent},
  {path: 'profile', component: ProfileComponent},
  {path: 'apiInfo', component: ApiInfoComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MainRoutingModule { }
