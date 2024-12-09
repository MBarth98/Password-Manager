This application allows you to securely store your credentials on a single device. It also helps you generate strong passwords. By focusing on both security and usability, it aims to simplify your password management without compromising safety.

# How to run the application

## Backend
1.  Set up a .NET 6 or later environment.
2. In appsettings.json, configure your DefaultConnection string, JWT_SECRET_KEY (or use envionment variable with the same name, more secure). 
3. Start the backend with dotnet run.

## Frontend
1. Use npm install to install dependencies.
2. Run ng serve to start the Angular development server.
3. Access the app at http://localhost:4200.

## Authentication & Master Password
1. Register a new user and log in.
2. The app derives a master key (in memory only) from your email and master password. Lose this session, and you’ll need to log in again (for getting the master password again).

## Screenshots

(Conceptual Only)

    Login screen: Enter email and master password.
    Main dashboard: Add, edit, view (show/hide), and delete stored passwords.

# Security Model discussion

### What are we protecting against?
We’re safeguarding your credentials from unauthorized access. Our primary threats include attackers who gain access to the backend database or intercept network traffic. We’re also wary of someone physically accessing your device without proper authentication. The idea is to ensure that even if the backend is compromised, attackers only get useless encrypted data, not your plaintext passwords.

## Security Model

### Backend Security:
* Key Handling: The backend never sees your master password or the derived master key. It stores only encrypted passwords and salts.
* Encryption: All encryption and decryption happen client-side. Your master key never leaves your browser’s memory.
* Password Hashing & Salting: User account passwords are hashed and salted server-side using a proven algorithm bcrypt. The backend never stores plaintext user credentials.
* JWTs (JSON Web Tokens): The server issues short-lived tokens to authenticate requests. Once the token expires, you must re-authenticate, ensuring that a lost or stolen token has limited value.
* CORS Configuration: The API restricts domains that can request resources, mitigating cross-site attacks.

### Access Control:
Your user ID is embedded in the JWT. The backend checks this ID against ownership of password entries. Only their rightful owner can view, modify, or delete stored credentials.

### Frontend Security:
The master key used to encrypt/decrypt passwords is derived once you log in and kept only in memory. Passwords are never stored in plaintext locally or on the server. They remain encrypted until you explicitly choose to show them. If the session expires, an attempt to show a password without a master key just informs you to re-log in—no plaintext is revealed.

## Pitfalls and limitation in security

### Client-Side Key Exposure:
If an attacker compromises your browser via malware or XSS, they might access the in-memory master key. You can mitigate this by keeping your system and browser secure and using a strong Content Security Policy.

### Session Management:
Once your master key is gone (e.g., after a refresh or session timeout), you must log in again. While this enhances security, it might reduce convenience.

### Backend Breach Scenario:
If the database is stolen, attackers only get encrypted passwords and salts. Without your master key, these remain unreadable. However, if they trick you into decrypting on a compromised device, they may gain plaintext..


In essence, this product’s security focuses on strong cryptography, minimal secret exposure, and careful session handling. While it’s not immune to all threats, it significantly raises the bar against common attacks and casual data breaches, encouraging users to adopt better password practices without making their lives harder.