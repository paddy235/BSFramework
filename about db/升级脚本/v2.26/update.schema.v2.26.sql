/****
本示例为SQL脚本更新模版
1.本模版为统一管理SQL脚本特别定制，模版提供添加表，添加列2个基本示例。
2.新增、修改、删除结构时，需要进行有效判断，避免产生执行SQL冲突，导致后续SQL语句执行不成功。
3.索引，视图，函数等参照2个基本示例进行添加
4.每个更新语句注明日期、人员、功能的简单说明
5.添加好SQL后，请运行，确保SQL整体运行通过
**/
/**SQL升级开始**/

/**判断指定数据库下的指定表，如果不存在时则创建**/
/**
日期：2019-04-23
人员：尚雄
备注：创建测试表
**/











/**
判断指定表中的列是否存在，不存在则添加
由于mysql不支持匿名块，所以语句需要放到存储过程或函数中进行
**/
/**
日期：2019-04-23
人员：尚雄
备注：添加测试列
**/
drop procedure if exists pro_AddColumn;
delimiter $$
create procedure pro_AddColumn() 
begin
declare tableschema  varchar(50);
set tableschema = 'bst-bzzd';
/**----------------------更新列写在本虚线范围内--------------------**/



if not exists(select 1 from information_schema.columns t where t.TABLE_SCHEMA = tableschema and table_name='wg_edubaseinfo' and upper(column_name) = 'MEETINGID') THEN
alter table wg_edubaseinfo add MEETINGID varchar(36);
END IF;

if not exists(select 1 from information_schema.STATISTICS t where t.TABLE_SCHEMA = tableschema and lower(table_name)='wg_edubaseinfo' and upper(column_name) = 'MEETINGID') THEN
CREATE INDEX IX_MEETINGID ON wg_edubaseinfo ( MEETINGID );
end if;

















/**-----------------------更新列写在本虚线范围内--------------------**/
end;$$
delimiter;
call pro_AddColumn;
drop procedure pro_AddColumn;

/**SQL升级结束**/



