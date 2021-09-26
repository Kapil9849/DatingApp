import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model :any={};
  currentUser$!: Observable<User>;
  // loggedIn!: boolean;
  
  constructor(public service: AccountService, private router:Router,
    private toastr: ToastrService) { }

  ngOnInit(): void {
    // this.getCurrentUser();
    this.currentUser$=this.service.currentUser$;
  }
  
  login()
  {
    
    this.service.login(this.model).subscribe(response => {
      this.router.navigateByUrl('/members');
      // this.loggedIn=true;
    });
  }


  logout()
  {
    this.service.logout();
    this.router.navigateByUrl('/')
    // this.loggedIn=false;
  }

  // getCurrentUser()
  // {
  //   this.service.currentUser$.subscribe(user=>
  //     {
  //       this.loggedIn=!!user;
  //     },error=>
  //     {
  //       console.log(error);
  //     })
  // }

  

}
