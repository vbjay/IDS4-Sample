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
1. SSL
    1. ```cd "c:\Program Files (x86)\IIS Express"```
    1. ```IisExpressAdminCmd.exe setupsslUrl -url:https://localhost:44310/ -UseSelfSigned```
    1. ```IisExpressAdminCmd.exe setupsslUrl -url:https://localhost:44302/ -UseSelfSigned```
    1. ```IisExpressAdminCmd.exe setupsslUrl -url:https://localhost:44303/ -UseSelfSigned```
    1. Debug and trust all certificate trust prompts

1. Switch directory

    ```cmd
        cd src\AdminUI.Admin
    ```

1. install node modules
    *had trouble with latest version of node(15.3) so I used [NVM](https://blog.logrocket.com/switching-between-node-versions-during-development/) to switch the version to LTS (14.15.1).*

    ```cmd
        npm i
    ```

1. run gulp

    ```cmd
        gulp
    ```

1. Update scripts/css

    1. Switch directory

        ```cmd
            cd ..\AdminUI.STS.Identity
        ```

    1. install node modules

        ```cmd
            npm i
        ```

    1. run gulp

        ```cmd
            gulp
        ```

1. See AdminUI.Admin\Sample Config\ folder for info on the clients and related items to set up.  Create the scopes and resources first in the admin ui.
    ![create clients](/assets/create%20clients.png)
    - Weather-swgager is a client credentials client, so use that template.
    - oidcClient is an authorization code client.  Use the server application template.
    ![create clients templates](/assets/create%20clients%20templates.png)
