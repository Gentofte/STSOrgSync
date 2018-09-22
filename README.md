# STSOrgSync


## Changelog

### 06.12.2016 Release 1.0.0
Initial release of STSOrgSync as installed in Gentofte, Syddjurs and Favrskov

### 21.12.2016 Release 1.0.1
* added more tests to BusinessLayer TestDriver
* increased limit on search-results to 5000 when querying Organisation (the undocumented default seems to be 500, which can be an issue in special cases)
* fixed bug, where moving an OrgUnit from one place in the hierarchy to another would fail to be updated correctly
* performance improvement on report tool

### 02.02.2017 Release 1.1.0
* added support for KOMBITs registration pattern 'Henvendelsessteder'
* added support for multiple positions for a single User
* added an official API for reading data from Organisation
* changed how objects are 'deleted' according to new version of KOMBITs registration requirements


### 25.01.2018 Release 1.2.0
* added support for new registration-practice regarding the Organisation object and the root OrganisationEnhed object
* added latest "registration practice" document from KOMBIT, and aligned code with guidelines
* updated code to use the latest version of the SOAP interface on the Serviceplatform
* support for PROD environment
* fixed wrong UUID on address objects
* fixed bug with database-queue, which might skip multiple registrations on same object

### 22.09.2018 Release 1.3.0
* as the STS Organisation service is now able to return "actual state" objects, we no longer maintain full object history on the latest registration
* updated with new UUID constants according to the latest "anvisninger" document
* various small fixes to deal with incomplete data
* can successfully deal with the interim-solution for super-org data, and deals with KOMBIT-owned super-org data without issues
* updated to latest WSDL/XSD published on the serviceplatform (v4)

#### License (in danish)

Copyright Gentofte Kommune/OS2 Offentlig Digitaliseringsfællesskab. Dette værk er licensieret under opensource
licensen Mozilla Public License 2.0. Læs mere i license.txt filen.
