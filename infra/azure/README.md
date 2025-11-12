# Emplyx Azure Infrastructure

This folder keeps the infrastructure-as-code (IaC) needed to stand up the cloud services that run the Emplyx workload inside the resource group `emplyx`. Everything is described with [Bicep](https://learn.microsoft.com/azure/azure-resource-manager/bicep/overview) so we can validate locally (what-if, lint, CI) and only provision real resources once we are ready.

## Topology

`infra/azure/main.bicep` is subscription-scoped and creates the `emplyx` resource group plus a single module (`modules/environment.bicep`) that deploys all resources for one environment (dev/qa/prod):

- **App Service Plan + Web App** (Linux, .NET 9) with a staging slot, autoscale rules, and telemetry wired to Application Insights.
- **Azure Functions** consumption plan (`func-<env>-worker`) hosting queue/topic driven jobs (auditoría, delegaciones, notificaciones).
- **Azure SQL** server/database (`emplyx`) for transactional data. The WebApp/Functions use managed identity and Key Vault stored connection strings.
- **Service Bus Standard namespace** with a durable topic (`user-events`) + subscriptions per concern and a queue (`audit-log`) for high-volume inserts.
- **Cosmos DB (SQL API)** database/container dedicated to auditoría documents (partition key `/tenantId`) with autoscale throughput.
- **Two Storage Accounts**: one shared (queues for audit/delegation/email workflows) and one for the Function host (AzureWebJobsStorage).
- **Key Vault** with RBAC enabled, purge protection, and role assignments so the managed identities can resolve Key Vault references from their app settings.
- **Log Analytics + Application Insights** plus diagnostic settings that stream logs/metrics from App Service, Functions, Service Bus, and Key Vault.

All secrets (SQL strings, Service Bus shared access key, Cosmos key, B2C client secret) are stored as Vault secrets. App settings only hold Key Vault references, so rotating keys is centralized.

## Local-first workflow

| Capability | Local/dev notes |
| --- | --- |
| Web App / APIs | Run `dotnet watch` from `Emplyx.WebApp`; slots are only needed in Azure. |
| Azure Functions | Use `func start` locally; the Function App created by IaC mirrors the same settings. |
| Storage Queues | Use [Azurite](https://learn.microsoft.com/azure/storage/common/storage-use-azurite) locally. Connection names (`StorageQueues__*`) match those in the WebApp/Functions settings. |
| Service Bus | There is no official emulator. Keep unit tests decoupled via interfaces; when needed point to a shared dev namespace (deploy the template against a dev subscription). |
| SQL / Cosmos | Continue using the local DB provider (e.g., SQL LocalDB, Cosmos emulator) while developing. The Bicep template guarantees the Azure shape is reproducible. |
| Identity (Azure AD B2C / CegidID) | Develop using ASP.NET Identity or test tenants; the template only stores the configuration placeholders so we can plug the real IdP later. |

## Parameters & secrets

- `main.bicep` exposes the small set of inputs we need (location, environment name, SKU choices, SQL admin, B2C info, etc.).
- Keep the provided `parameters/main.dev.json` file as a **template only**. Copy it (e.g., `main.dev.local.json`), fill the placeholders, and never commit real secrets.
- Secure parameters (`sqlAdministratorPassword`, `b2cClientSecret`) are marked with `@secure()` so Azure will treat them as secrets even when provided via CLI/CI variables.

## Deploy / validate

1. **Prerequisites**
   ```bash
   az login
   az account set --subscription "<SUBSCRIPTION_ID>"
   az bicep install           # only once
   ```

2. **What-if / dry run**
   ```bash
   az deployment sub what-if \
     --name emplyx-dev \
     --location westeurope \
     --template-file infra/azure/main.bicep \
     --parameters @infra/azure/parameters/main.dev.json
   ```

3. **Deploy**
   ```bash
   az deployment sub create \
     --name emplyx-dev \
     --location westeurope \
     --template-file infra/azure/main.bicep \
     --parameters @infra/azure/parameters/main.dev.json
   ```

Because the entry-point is subscription-scoped, the template will create/update the `emplyx` resource group automatically and deploy every service within it.

## Post-deployment checklist

1. **SQL AAD admin** – Set an Azure AD admin on the SQL server (`az sql server ad-admin ...`) so managed identities can request tokens. The template only creates the SQL login/password for emergency use.
2. **Key Vault RBAC** – If operators need to read secrets, assign them the `Key Vault Secrets Officer` role at the vault scope.
3. **Azure AD B2C / CegidID configuration** – Register the Emplyx app in the IdP, then populate the `b2cTenant`, `b2cClientId`, `b2cClientSecret`, and `cegidIdAuthority` parameters before deploying to higher environments.
4. **Service connections (CI/CD)** – In Azure DevOps/GitHub Actions, create federated credentials for the deployment service principal and use the same Bicep template from pipelines.
5. **Observability** – The template already streams platform logs to Log Analytics. Configure Application Insights alerts/SLOs that reflect Emplyx KPIs (latency, Service Bus dead letters, Function failures, etc.).

## Extending the template

- Add more Function Apps or WebJobs by cloning the existing module sections (e.g., a dedicated notification worker).
- Introduce optional resources (Redis cache, Azure SignalR, etc.) by adding new parameters and resource blocks in `environment.bicep`.
- If we need multiple regions, wrap the module invocation in `main.bicep` within a loop per region/resource group.

Having the entire azure footprint codified here lets us iterate locally (lint, `what-if`, unit tests against emulators) and promote to Azure with confidence when production is ready.
