certutil.exe -addstore TrustedPeople sts.cer
certutil.exe -addstore TrustedPeople organisation.cer
certutil.exe -addstore root oces-test-root-ca.cer
certutil.exe -addstore ca oces-test-intermedicate-ca.cer
certutil.exe -addstore TrustedPeople serviceplatform.cer
