export interface Post {
    id?: number;
    createrId: number;
    createrPhotoUrl?: string;
    createrUsername: string;
    createrKnownAs: String;
    content: string;
    postCreated: Date;
}