import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
// import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { MainModule } from './Modules/main/main.module';
import { AuthModule } from './Modules/auth/auth.module';
import { AuthComponent } from './Modules/auth/auth.component';
import { MainComponent } from './Modules/main/main.component';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { NavbarComponent } from './Modules/Components/navbar/navbar.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ToastrModule } from 'ngx-toastr';
import { ProgressbarModule } from 'ngx-bootstrap/progressbar';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { LoggingInterceptor } from './Modules/Components/interceptor/logging.interceptor';
import { JwtInterceptor } from './Modules/Components/interceptor/jwt-interceptor.interceptor';
import {MatFormFieldModule} from '@angular/material/form-field';
import { DatePipe } from '@angular/common';
import {MatTableModule} from '@angular/material/table';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgxFontAwesomeModule } from 'ngx-font-awesome';


@NgModule({
  declarations: [
    AppComponent,
    AuthComponent,
    MainComponent,
    NavbarComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    AppRoutingModule,
    // NgxFontAwesomeModule
    // BrowserAnimationsModule,
    // FontAwesomeModule,
    BrowserAnimationsModule,
    BsDropdownModule.forRoot(),
    CollapseModule.forRoot(),
    ButtonsModule.forRoot(),
    ModalModule.forRoot(),
    HttpClientModule,
    // NgModule,
    FormsModule,
    ToastrModule.forRoot(),
    ProgressbarModule.forRoot(),
    MatFormFieldModule,
    MatTableModule,
    NgSelectModule,
    NgxFontAwesomeModule,
    // ProgressbarModule.forRoot()
    // AuthModule,
    // MainModule
  ],
  providers: [
    DatePipe,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoggingInterceptor,
       multi: true
      },
      {
        provide: HTTP_INTERCEPTORS,
        useClass: JwtInterceptor,
         multi: true
        },
    ],
  bootstrap: [AppComponent]
})
export class AppModule { }
