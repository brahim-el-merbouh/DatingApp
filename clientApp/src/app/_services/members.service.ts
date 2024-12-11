import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Member } from '../_models/member';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http =inject(HttpClient)

  getMembers() {
    return this.http.get<Member[]>(`${environment.baseApiUrl}users`);
  }

  gteMember(username: string) {
    return this.http.get<Member>(`${environment.baseApiUrl}users/${username}`);
  }
  
}
