# Admin UI

See [skoruba/IdentityServer4.Admin](https://github.com/skoruba/IdentityServer4.Admin)

## Actions

1. Install template

    ```cmd
        dotnet new -i Skoruba.IdentityServer4.Admin.Templates::2.0.0-beta1
    ```

1. Create projects

    ```cmd
        dotnet new skoruba.is4admin --name AdminUI --title "Sample Admin UI" --adminemail "admin@example.com" --adminpassword "Pa$$word123" --adminrole Administrator --adminclientid AdminClient --adminclientsecret AdminClientSecret --dockersupport false
    ```

1. Set the admin ui projects to start
  ![multi debug admin ui](/assets/multi%20debug%20admin%20ui.png)
