import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'client';
  users: any;

  constructor(private accountservice: AccountService){}

  ngOnInit() {
    
    this.setCurrentUser();
  }

  setCurrentUser()
  {
    const userjson=localStorage.getItem('user');
    const user: User=JSON.parse(userjson!);
    this.accountservice.setCurrentUser(user);
  }

  
}
