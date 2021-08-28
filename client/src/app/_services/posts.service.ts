import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { Message } from '../_models/message';
import { Post } from '../_models/post';
import { PostLikes } from '../_models/postLikes';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class PostsService {

  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private postLikes = new BehaviorSubject<PostLikes>(null);
  likeThread$ = this.postLikes.asObservable();


  constructor(private toastr: ToastrService, private http: HttpClient) { }



  createHubConnection(user: User, member: Member, post: Post) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'post?postId=' + post.id + '&userId=' + member.id, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build()

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('NewLikeDone', postlike => {
      this.postLikes.next(postlike);
      this.toastr.info(' Post liked Successfully!')
      this.postLikes = null;
      this.likeThread$ = null;
    })


    this.hubConnection.on('NewLikeReject', () => {
      this.toastr.warning('You Had Liked the post ')
    })

  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  addPost(content: string) {
    return this.http.post(this.baseUrl + 'posts/add-post/' + content, {});
  }

  getPosts(pageNumber, pageSize, core) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Core', core);
    return getPaginatedResult<Partial<Post[]>>(this.baseUrl + 'posts/get-posts/', params, this.http);
  }


  deletePost(postId: number) {
    return this.http.delete(this.baseUrl + 'posts/delete-post/' + postId);
  }

  addLikeToPost(username: string) {
    return this.http.post(this.baseUrl + 'posts/' + username, {})
  }

  getLikesOfPost(postId: number) {
    return this.http.get<PostLikes[]>(this.baseUrl + 'postLikes/' + 'users-liked-posts/' + postId);
  }

  getPostLiked(member: Member, pageNumber, pageSize) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    const userId = member.id;
    const userName = member.username;
    params = params.append('UserId', userId);
    params = params.append('Username', userName);

    return getPaginatedResult<Partial<Post[]>>(this.baseUrl + 'postLikes/' + 'user-likes', params, this.http);
  }
}
