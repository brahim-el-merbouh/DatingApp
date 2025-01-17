import { Component, inject, input, OnInit, output } from '@angular/core';
import { Member } from '../../_models/member';
import { DecimalPipe, NgClass, NgFor, NgIf, NgStyle } from '@angular/common';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { AccountService } from '../../_services/account.service';
import { environment } from '../../../environments/environment';
import { Photo } from '../../_models/photo';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-photo-editor',
  standalone: true,
  imports: [NgIf, NgFor, NgClass, NgStyle, FileUploadModule, DecimalPipe],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css'
})
export class PhotoEditorComponent implements OnInit {
  member = input.required<Member>();
  private accountService = inject(AccountService);
  private memberService = inject(MembersService)
  uploader?: FileUploader;
  hasBaseDropZoneOver = false;
  memberChange = output<Member>();

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  deletePhoto(photo: Photo){
    this.memberService.deletePhoto(photo).subscribe({
      next: _ => {
        const updatedMember = {...this.member()};
        updatedMember.photos = updatedMember.photos.filter(x => x.id != photo.id);
        this.memberChange.emit(updatedMember);
      }
    });
  }

  setMainPhoto(photo: Photo){
    this.memberService.setMainPhoto(photo).subscribe({
      next: _ => {
        const updatedMember:Member = {...this.member()};
        updatedMember.photos.push(photo);
        this.UpdateMemberWithPhoto(photo, updatedMember);
        this.memberChange.emit(updatedMember);
      }
    });
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: `${environment.baseApiUrl}users/add-photo`,
      authToken: `Bearer ${this.accountService.currentSuer()?.token}`,
      removeAfterUpload: true,
      autoUpload:false,
      maxFileSize: 1 * 1024 * 1024
    });
    
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      const photo = JSON.parse(response);
      const updatedMember:Member = {...this.member()};
      updatedMember.photos.push(photo);
      if (photo.isMain) {
        this.UpdateMemberWithPhoto(photo, updatedMember);
      }
      this.memberChange.emit(updatedMember);
    };

  }


  private UpdateMemberWithPhoto(photo: any, updatedMember: Member) {
    
      const user = this.accountService.currentSuer();
      if (user) {
        user.photoUrl = photo.url;
        this.accountService.setCurrentUser(user);
      }

      updatedMember.photoUrl = photo.url;
      updatedMember.photos.forEach(p => {
        if (p.isMain) p.isMain = false;
        if (p.id === photo.id) p.isMain = true;
      });
      
    
  }
}
