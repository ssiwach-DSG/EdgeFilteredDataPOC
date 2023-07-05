# Edge-Poc-FilteredProductData
 This Azure Function is triggered when a new file is received in blob stoarage, and push the content to Redis Instance

az functionapp create --resource-group rg-product-catalog-np --consumption-plan-location eastus --runtime dotnet-isolated --functions-version 4 --name EdgeFilteredDataPOC --storage-account stdsgpcedgedev


 //"AzureWebJobsStorage__blobServiceUri": "https://stdsgpcedgedev.blob.core.windows.net/edge-sku-storage?sp=r&st=2023-06-09T14:27:16Z&se=2023-06-09T22:27:16Z&spr=https&sv=2022-11-02&sr=c&sig=UUr3l3zQu%2BY8WHrb8uf2nONAZJyAGwo5BUDirXfVAVo%3D",


     "AzureWebJobsStorage": "DefaultEndpointsProtocol=[http|https];AccountName=stdsgpcedgedev;AccountKey=sp=r&st=2023-06-09T14:30:13Z&se=2023-06-09T22:30:13Z&sv=2022-11-02&sr=c&sig=pDZ7r3rW6OoPZlrb0aLPDOToTz37nJWo1unoRwrtRXw%3D",
    

https://github.com/ssiwach-DSG/EdgeFilteredDataPOC

func azure functionapp publish pde-edge-filtereddata-poc

func azure functionapp publish ProductFilterdData-EdgeFunction

func azure functionapp publish pde-poc-load-test

func azure functionapp publish pdepoc2

