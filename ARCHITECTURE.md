* Group access is controlled via database relation. Multi-service access control could be achieved with authorization claims/tokens and skipping an internal service db check, 
* but this would come at the cost of either having to track and revoke tokens on group access revocation, or re-authenticate on group add. 
* Assuming groups change frequently, the database seems a better fit.

* Documents are stored as pointers to a resource location. BLOBs are a (slow) option, but probably best delegated to a separate database if chosen.

* TODO 
* OIDC
* S3
* Vault/SecretsManager