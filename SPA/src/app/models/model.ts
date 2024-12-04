export interface PasswordEntry {
  website: string;
  username: string;
  salt: string;
  encryptedPassword: string;
}
