# Episerver Commerce Fundamental Course Solution Files
## General Information
This is just a site for exercises during "the Commerce Fundamentals" course.  The code in this solution is written
for training purposes and is **not intended for production use**.

For best practices and references... have look at [Quicksilver on GitHub](https://github.com/episerver/Quicksilver).

The front-end CMS site is configured for - http://localhost:57759/

The back-end Commerce Manager site is configured for - http://localhost:57878/

## Setup Instructions
To avoid having to edit any configuration files the solution needs to sit in the folder path of 
**C:\Episerver\CommerceTraining**. Note that there is a **CommerceTraining** subfolder that stores the front-end
site project. The following image shows what the folder structure should look like:

![Folder Structure](/Images/folder-structure.gif)

If you do choose to clone or unzip into a different folder path there are a couple of configuration files
you will need to modify.  The fist file you would modify is the **Web.config** file in the **CommerceMananger**
project, see the following image.

![Web.config circled](/Images/webconfig-circled.gif)

Look for the **connectionStrings** section, it should look like the following:
```xml
  <connectionStrings>
    <add name="EPiServerDB" connectionString="Data Source=(LocalDb)\MSSQLLocalDB;AttachDbFilename=C:\Episerver512\CommerceTraining\CommerceTraining\App_Data\EPiServerDB_3a678c46.mdf;Initial Catalog=EPiServerDB_3a678c45;Connection Timeout=60;Integrated Security=True;MultipleActiveResultSets=True" providerName="System.Data.SqlClient" />
    <add name="EcfSqlConnection" connectionString="Data Source=(LocalDb)\MSSQLLocalDB;AttachDbFilename=C:\Episerver512\CommerceTraining\CommerceTraining\App_Data\EcfSqlConnection_d3a95dd5.mdf;Initial Catalog=EcfSqlConnection_d3a95dd4;Connection Timeout=60;Integrated Security=True;MultipleActiveResultSets=True" providerName="System.Data.SqlClient" />
  </connectionStrings>
  ```
Modify the two **AttachDbFilename** attribute paths to reflect whatever path your local install is using and save the file.   The second
file you need to modify is the **Mediachase.Search.config** file located in the **CommerceTraining** project under the **Configs** folder, see
the following image:

![Mediachase.Search.config circled](/Images/mediachase-search-config-circled.gif)

Look for the **add** element with the **name** attribute set to "LuceneSearchProvider", it should look like the following:
```xml
<add name="LuceneSearchProvider" type="Mediachase.Search.Providers.Lucene.LuceneSearchProvider, Mediachase.Search.LuceneSearchProvider" queryBuilderType="Mediachase.Search.Providers.Lucene.LuceneSearchQueryBuilder, Mediachase.Search.LuceneSearchProvider" storage="C:\Episerver512\CommerceTraining\CommerceMananger\App_Data\Search\ECApplication\" simulateFaceting="true" />
```
Modify the **storage** attribute path to reflect whatever path your local install is using and save the file.