create table #dates (DT DATETIME);

insert into #dates (DT) VALUES
('2017-02-01 15:00:43'),
('2017-03-18 10:30:10'),
('2017-12-11 01:21:55'),
('2017-12-11 15:33:03'),
('2017-03-18 10:30:10'),
('2017-02-22 08:40:42'),
('2017-06-06 23:59:40');

--�.�. � ������� ������ ��� �������� �� ������ MS SQL Server'a, ��������� Lead, ������ ��� ������� �������, �� ��� ������� OVER
select 
D.DT,
LEAD(D.DT, 1, NULL) OVER(ORDER BY DT)
from #dates AS D
order by DT


-- ��������� ����������� ��-�� 2017-03-18 10:30:10.000 (������������� ��������)
--select T.DT, (select TOP 1 DT from #dates where DT > T.DT order by DT)
--from #dates as T
--order by T.DT


--����� �� �� �������������� ������ - ��� ���� ��������� ������������ ��� 2017-03-18 10:30:10.000
--select T.DT, 
--(select 
--	case when exists(select count(DT) from #dates where DT = T.DT HAVING COUNT(DT) > 1)
--	 then DT
--	 else (select TOP 1 DT from #dates where DT > T.DT order by DT)
--	 end
--)
--from #dates as T
--order by T.DT


drop table #dates


