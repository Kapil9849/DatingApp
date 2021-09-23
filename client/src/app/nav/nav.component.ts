import { Component, OnInit } from '@angular/core';
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
  
  constructor(public service: AccountService) { }

  ngOnInit(): void {
    // this.getCurrentUser();
    this.currentUser$=this.service.currentUser$;
  }
  
  login()
  {
    
    this.service.login(this.model).subscribe(response => {
      console.log(response);
      // this.loggedIn=true;
    },error=>{
      console.log("Error Occured : ",error);
    });
  }


  logout()
  {
    this.service.logout();
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
