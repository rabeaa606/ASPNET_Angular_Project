import { Component, OnInit, ViewChild } from '@angular/core';
import { Pagination } from '../_models/pagination';
import { Post } from '../_models/post';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { ToastrService } from 'ngx-toastr';
import { NgForm } from '@angular/forms';
import { Member } from '../_models/member';
import { PostsService } from '../_services/posts.service';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';

@Component({
  selector: 'app-posts',
  templateUrl: './posts.component.html',
  styleUrls: ['./posts.component.css']
})
export class PostsComponent implements OnInit {
  @ViewChild('postsTabs', { static: true }) postsTabs: TabsetComponent;
  activeTab: TabDirective;

  @ViewChild('postForm') postForm: NgForm;

  posts: Partial<Post[]>;
  pageNumber = 1;
  pageSize = 5;
  mypagination: Pagination;
  otherpagination: Pagination;
  likedPostspagination: Pagination;

  member: Member;
  user: User;
  content: string = "";
  myPosts: Post[] = [];
  LikedPosts: Post[] = [];
  core = 'My';

  constructor(private postsService: PostsService,
    private membersService: MembersService,
    public accountService: AccountService,
    private toastr: ToastrService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  async ngOnInit() {
    await this.loadMember();
  }

  async onTabActivated(data: TabDirective) {
    this.activeTab = data;
    switch (this.activeTab.heading) {
      case "MY Postss":
        await this.loadMyPosts();
        break;
      case "Others Posts":
        await this.loadPosts();
        break;
      case "Posts I Like":
        this.getLikes();
        break;

    }
  }

  loadMyPosts() {
    this.core = "My";
    this.postsService.getPosts(this.pageNumber, this.pageSize, this.core).subscribe(response => {
      this.myPosts = response.result;
      this.mypagination = response.pagination;
    })
  }

  loadPosts() {
    this.core = "Others";
    this.postsService.getPosts(this.pageNumber, this.pageSize, this.core).subscribe(response => {
      this.posts = response.result;
      this.otherpagination = response.pagination;
    })
  }

  myPostsPageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadMyPosts();
  }

  otherPostsPageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadPosts();
  }

  likedPostspaginationPageChanged
    (event: any) {
    this.pageNumber = event.page;
    this.getLikes();
  }

  async loadMember() {
    await this.membersService.getMember(this.user.username).subscribe(member => {
      this.member = member;

    })
  }
  createPost() {
    const post: Post = {
      createrId: this.member.id,
      content: this.content,
      createrKnownAs: this.member.knownAs,
      createrUsername: this.member.username,
      createrPhotoUrl: this.member.photoUrl,
      postCreated: new Date(Date.now()),
    };

    this.postsService.addPost(this.content).subscribe(() => {
      this.myPosts.unshift(post);
      this.toastr.success('Post Submited successfully ');
      this.postForm.reset();
    })
  }

  deletePost(post: Post) {
    this.postsService.deletePost(post.id).subscribe(() => {
      this.myPosts.splice(this.myPosts.indexOf(post), 1)
    })
  }
  getLikes() {
    this.postsService.getPostLiked(this.member, this.pageNumber, this.pageSize).subscribe(response => {
      this.LikedPosts = response.result;
      this.likedPostspagination = response.pagination;
    })
  }
}

