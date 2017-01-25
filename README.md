# STSOrgSync


## Changelog

### 06.12.2016 Release 1.0.0
Initial release of STSOrgSync as installed in Gentofte, Syddjurs and Favrskov

### 21.12.2016 Release 1.0.1
* added more tests to BusinessLayer TestDriver
* increased limit on search-results to 5000 when querying Organisation (the undocumented default seems to be 500, which can be an issue in special cases)
* fixed bug, where moving an OrgUnit from one place in the hierarchy to another would fail to be updated correctly
* performance improvement on report tool

#### License (in danish)

Copyright Gentofte Kommune/OS2 Offentlig Digitaliseringsfællesskab. Dette værk er licensieret under opensource
licensen Mozilla Public License 2.0. Læs mere i license.txt filen.