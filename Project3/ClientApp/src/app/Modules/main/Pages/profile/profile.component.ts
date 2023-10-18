import { Component, OnInit } from '@angular/core';
import { AccountService } from 'src/app/Modules/auth/Services/account.service';
// import {
  // faEdit,
  // faCheck,
  // faX,
  // faLeaf
// } from '@fortawesome/free-solid-svg-icons';
import { ProfileService } from '../../Services/profile.service';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit{
  // facheck = faCheck;
  // faedit = faEdit;
  // fax = faX;
  currentName : string;
  currentEmail : string;
  isEditable : boolean = false;
  isPassEdit : boolean = false;
  isEditablePass : boolean = false;
  currentPassword : string;
  tempNewPass : string;
  newPass : string;
  ngOnInit(): void {
    // throw new Error('Method not implemented.');
    this.getValue();
  }
  constructor(
    private authService : AccountService,
    private profileService : ProfileService,
    private toastr : ToastrService
  ){ }
  getValue(){
    const currentUser = this.authService.currentUserSource.subscribe(res => {
      console.log(res);
      if(res != null){
        this.currentName = res.displayName;
        this.currentEmail = res.email;
      }
    })
    console.log(this.currentEmail, this.currentName);
  }
  toggleEdit(){
    this.isEditable = !this.isEditable;
  }
  submitEdit(){
    var funBody = {
      "Email": this.currentEmail,
      "DisplayName": this.currentName
    }
    console.log(funBody);
    this.profileService.updateProfile(funBody)
      .subscribe(res => {
        console.log(res);
        this.isEditable = false;
        this.toastr.success("Update Successful");
      }, error => {
        console.log(error);
      })
    this.getValue();
  }
  toggleEditPassword(){
    this.currentPassword = '';
    this.tempNewPass = '';
    this.newPass = '';
    // this.isEditable = !this.isEditable;
    this.isPassEdit = !this.isPassEdit;
    this.isEditablePass = !this.isEditablePass;
  }

  submitPass(){
    var funBody = {
      "currentPassword": this.currentPassword,
      "newpassword": this.newPass
    }
    console.log(this.currentPassword, this.tempNewPass, this.newPass);
    if(this.tempNewPass != this.newPass){
      this.toastr.warning("Your New Passwords Doesn't Match, Please Try Again");
    }else{
      this.profileService.changePassword(funBody)
        .subscribe((res) => {
          console.log(res);
          this.toggleEditPassword();
          this.toastr.success("Password Changed Successfully");
        }, error => {
          console.log(error);
          this.toastr.error(error.error.message);
        })
    }
  }
}
