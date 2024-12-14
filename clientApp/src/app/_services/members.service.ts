import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Member } from '../_models/member';
import { environment } from '../../environments/environment';
import { of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http =inject(HttpClient)
  members = signal<Member[]>([]);

  getMembers() {
    return this.http.get<Member[]>(`${environment.baseApiUrl}users`).subscribe({
      next: members => this.members.set(members)
    });
  }

  gteMember(username: string) {
    const member = this.members().find(x => x.username === username);
    if (member !== undefined) return of(member);

    return this.http.get<Member>(`${environment.baseApiUrl}users/${username}`);
  }
  
  updateMember(member:Member){
    return this.http.put(`${environment.baseApiUrl}users`,member).pipe(
      tap(() => {
        this.members.update(members => members.map(m => m.username === member.username 
          ? member : m))
      })
    );
  }
}
