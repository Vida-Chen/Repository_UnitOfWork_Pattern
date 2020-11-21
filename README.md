## Repository pattern & UnitOfWork pattern
**Repository.cs** - 針對單一自訂物件操作CRUD用
**RepositoryTransaction.cs** - 針對多物件的CRUD控管交易機制(transaction)一致性用

#### 測試與結果：
- Repository testing
  + Insert
  ![](https://github.com/Vida-Chen/Repository_UnitOfWork_Pattern/blob/master/test/insert.jpg?raw=true)
  ![](https://github.com/Vida-Chen/Repository_UnitOfWork_Pattern/blob/master/test/insert_result.jpg?raw=true)
  + Select and then Update  
  ![](https://github.com/Vida-Chen/Repository_UnitOfWork_Pattern/blob/master/test/select_update.jpg?raw=true)
  ![](https://github.com/Vida-Chen/Repository_UnitOfWork_Pattern/blob/master/test/select_update_result.jpg?raw=true)
  + Delete
  ![](https://github.com/Vida-Chen/Repository_UnitOfWork_Pattern/blob/master/test/delete.jpg?raw=true)
  ![](https://github.com/Vida-Chen/Repository_UnitOfWork_Pattern/blob/master/test/delete_result.jpg?raw=true)

- RepositoryTransaction testing
  + Insert
  ![](https://github.com/Vida-Chen/Repository_UnitOfWork_Pattern/blob/master/test/insert_trans.jpg?raw=true)
  ![](https://github.com/Vida-Chen/Repository_UnitOfWork_Pattern/blob/master/test/insert_trans_proj.jpg?raw=true)
  ![](https://github.com/Vida-Chen/Repository_UnitOfWork_Pattern/blob/master/test/insert_trans_mile.jpg?raw=true)
  + Select and then Update  
  ![](https://github.com/Vida-Chen/Repository_UnitOfWork_Pattern/blob/master/test/select_update_trans.jpg?raw=true)
  ![](https://github.com/Vida-Chen/Repository_UnitOfWork_Pattern/blob/master/test/select_update_trans_proj_result.jpg?raw=true)
  ![](https://github.com/Vida-Chen/Repository_UnitOfWork_Pattern/blob/master/test/select_update_trans_mile_result.jpg?raw=true)
  + Delete
  ![](https://github.com/Vida-Chen/Repository_UnitOfWork_Pattern/blob/master/test/select_delete.jpg?raw=true)

