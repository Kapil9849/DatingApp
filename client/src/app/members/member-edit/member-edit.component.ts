import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

  @ViewChild('editForm') editForm!:NgForm
  member!:Member;
  userd!:User;
  @HostListener('window:beforeunload',['$event']) unloadNotificaton($event:any)
  {
    if(this.editForm.dirty)
    {
      $event=true;
    }
  }


  constructor(private aservice:AccountService,private mservice:MembersService,
    private toastr: ToastrService) { 
    this.aservice.currentUser$.pipe(take(1)).subscribe(user=>
      {
        this.userd=user;
        console.log(this.userd.userName);
      })
  }

  ngOnInit(): void {
    this.LoadMember();
  }

  LoadMember()
  {
    this.mservice.getMember(this.userd.userName).subscribe(member=>{
      this.member=member;
      console.log(this.member);
    })
  }

  updateMember()
  {
    console.log(this.member);
    this.mservice.updateMember(this.member).subscribe(()=>{
      this.toastr.success("Profile Updated Successfully!");
      this.editForm.reset(this.member);
    })
    
  }

}
