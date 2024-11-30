import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { User } from '../_models/user';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private http = inject(HttpClient)
  currentSuer = signal<User | null>(null)
  
  login(model: any){
    return this.http.post<User>(`${environment.baseApiUrl}account/login`,model).pipe(
      map(user =>  {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentSuer.set(user);
        }
      })
    );
  }

  register(model: any){
    return this.http.post<User>(`${environment.baseApiUrl}account/register`,model).pipe(
      map(user =>  {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentSuer.set(user);
        }
        return user;
      })
    );
  }

  logout() {
    localStorage.removeItem('user');
    this.currentSuer.set(null);
  } 
}
