use FlexERPDW;

DECLARE @cnt int;
DECLARE @tmp varchar(500);
DECLARE @tmpdate datetime;
DECLARE @datavenc datetime;
DECLARE @dateemissao datetime;
DECLARE @rand int;
DECLARE @date datetime;
DECLARE @formacnt int;
DECLARE @empresacnt int;
DECLARE @clientecnt int;
DECLARE @titulocnt int;
declare @clascnt int;
declare @cgrp int = 1;
declare @csgrp int = 1;
declare @cclas int = 1;
declare @ctmp int;
declare @ctmp2 int;
declare @ctmp3 varchar(30);
declare @p1 varchar(4);
declare @p2 varchar(4);
declare @p3 varchar(4);
declare @tittmp varchar(30);
declare @vlrtmp float;
declare @empresatmp int;
declare @formatmp int;
declare @clientetmp int;
declare @desctmp float;
declare @enctmp float;
declare @vlrbxtmp float;

while (@@TRANCOUNT > 0)
begin
  rollback transaction;
end

set @cnt = 0;

begin transaction;
delete from Contas_a_Pagar;
delete from Contas_a_Receber;
delete from FormaPagtoDim;
delete from EmpresaDim;
delete from TituloCPDim;
delete from TituloCRDim;
delete from clientedim;
delete from ClassificaoFinanDim;
commit transaction;

insert into FormaPagtoDim (PK_FormaPagto, FormaPagamento) values (1,'Financiamento');
insert into FormaPagtoDim (PK_FormaPagto, FormaPagamento) values (2,'Cheque');
insert into FormaPagtoDim (PK_FormaPagto, FormaPagamento) values (3,'Dinheiro');
insert into FormaPagtoDim (PK_FormaPagto, FormaPagamento) values (4,'Cartão de crédito');
insert into FormaPagtoDim (PK_FormaPagto, FormaPagamento) values (5,'Troca/Devolução');
insert into FormaPagtoDim (PK_FormaPagto, FormaPagamento) values (6,'Desconto duplicada');
insert into FormaPagtoDim (PK_FormaPagto, FormaPagamento) values (7,'Crédito/Adiantamento');
select @formacnt = COUNT(*) from FormaPagtoDim;

insert into EmpresaDim (PK_Empresa, Nome) values (1, 'Filial SP');
insert into EmpresaDim (PK_Empresa, Nome) values (2, 'Filial MG');
insert into EmpresaDim (PK_Empresa, Nome) values (3, 'Filial RJ');
insert into EmpresaDim (PK_Empresa, Nome) values (4, 'Filial PR');
select @empresacnt = COUNT(*) from EmpresaDim;

begin transaction;
set @cnt = 1;
declare @regiaocliente varchar(20);
declare @ufcliente varchar(2);
while (@cnt < 1500)
begin
	set @ctmp = cast((RAND() * 100 ) as int);
	if (@ctmp > 66)
	begin
	    set @regiaocliente= 'Sudeste';
		set @ctmp = cast((RAND() * 100 ) as int);
		if (@ctmp > 60)
		   set @ufcliente = 'SP';
		else if (@ctmp > 30)
		   set @ufcliente= 'MG';
		else
		   set @ufcliente = 'RJ';   
	end
	else if (@ctmp > 25)
	begin
	    set @regiaocliente= 'Sul';
		set @ctmp = cast((RAND() * 100 ) as int);
		if (@ctmp > 60)
		   set @ufcliente = 'RS';
		else if (@ctmp > 30)
		   set @ufcliente= 'PR';
		else
		   set @ufcliente = 'SC';   
	end
    else 
    begin
	    set @regiaocliente= 'Centro-oeste';
		set @ctmp = cast((RAND() * 100 ) as int);
		if (@ctmp > 60)
		   set @ufcliente = 'GO';
		else if (@ctmp > 45)
		   set @ufcliente= 'TO';
		else if (@ctmp > 25)
		   set @ufcliente = 'MS';   
		else 
		   set @ufcliente = 'MT';   
    end; 
     
	insert into clientedim(pk_cliente, nome, classe, uf, regiao) values
		(@cnt, 'Cliente ' + cast(@cnt as varchar), 
		case when RAND() > 0.5 then 'Juridica' else 'Fisica' end,
		@ufcliente,
		@regiaocliente);
	set @cnt = @cnt + 1;
end
select @clientecnt = COUNT(*) from clientedim;
commit transaction;

begin transaction;
set @cnt = 1;
while (@cnt < 15000)
begin
	insert into TituloCPDim(PK_TituloCP) values
		('CP00' + cast(@cnt as varchar))
	insert into TituloCRDim(PK_TituloCR) values
		('CR00' + cast(@cnt as varchar))
	set @cnt = @cnt + 1;
end
select @titulocnt = COUNT(*) from TituloCPDim;
commit transaction;

begin transaction;
while (@cgrp <= 4)
begin
	set @ctmp = Cast((RAND() * (10)) AS INT) + 1;
	set @csgrp=1;
	while (@csgrp <= @ctmp)
	begin
		set @ctmp2 = CAST((RANd() * 15) AS INT) + 1;
		set @cclas=1;
		while (@cclas <= @ctmp2)
		begin
			set @p1 = cast(@cgrp as varchar);
			set @p1 = REPLICATE('0', 2 - LEN(@p1)) + @p1;
			set @p2 = CAST(@csgrp as varchar);
			set @p2 = REPLICATE('0', 3 - len(@p2)) + @p2;
			set @p3 = CAST(@cclas as varchar);
			set @p3 = REPLICATE('0', 4-len(@p3)) + @p3;
			set @ctmp3= @p1+'.'+@p2+'.'+@p3;
			
			insert into ClassificaoFinanDim (PK_ClassificacaoFinan, Grupo, Subgrupo, Classificacao, Grupo_Name, Subgrupo_Name, Classificacao_Name)
				values (@ctmp3, @p1, @p1 + '.' + @p2, @ctmp3, 'Grupo ' + @p1, 'Subgrupo ' +@p1 + '.' + @p2, 'Classificacao ' + @ctmp3);
				
        	set @cclas = @cclas +1;
		end
		set @csgrp  = @csgrp + 1;
	end
	set @cgrp = @cgrp + 1;
end	
select @clascnt= COUNT(*) from ClassificaoFinanDim;
commit transaction;

begin transaction;
set @cnt = 1;
while (@cnt < 15000)
begin
	set @tmp = CAST(Cast((RAND() * (2012 - 2004))+2004 AS INT) as varchar(4))
		+ '-' + CAST(Cast((RAND() * (12 - 1))+1 AS INT) as varchar(2))
		+ '-' + CAST(Cast((RAND() * (28 - 1))+1 AS INT) as varchar(2));
	set @datavenc =CAST(@tmp as datetime);
	
	set @tmp = CAST(Cast((RAND() * (2012 - 2004))+2004 AS INT) as varchar(4))
		+ '-' + CAST(Cast((RAND() * (12 - 1))+1 AS INT) as varchar(2))
		+ '-' + CAST(Cast((RAND() * (28 - 1))+1 AS INT) as varchar(2));
	set @dateemissao = CAST(@tmp as datetime);
	
	if (@datavenc < @dateemissao)
	begin
		set @tmpdate=@dateemissao;
		set @dateemissao=@datavenc;
		set @datavenc = @dateemissao;
	end

	set @cclas = Cast((RAND() * (@clascnt)) + 1 AS INT);
	with orderedclass as
    (
      select f.PK_ClassificacaoFinan, 
      ROW_NUMBER() over (order by PK_ClassificacaoFinan) as 'RowNumber'
      from ClassificaoFinanDim f
    )
	select @ctmp3 = oc.PK_ClassificacaoFinan
     from orderedclass oc
     where oc.RowNumber=@cclas;
     
     set @vlrtmp = ROUND( (6000 * RAND()) / (RAND() + 0.01), 2, 10);
     set @tittmp = 'CP00' + cast(Cast((RAND() * (@titulocnt)) + 1 AS INT) as varchar);
     
     set @formatmp= Cast((RAND() * (@formacnt)) + 1 AS INT);
	 set @empresatmp=Cast((RAND() * (@empresacnt)) + 1 AS INT);
	set @clientetmp=Cast((RAND() * (@clientecnt)) + 1 AS INT);
     
     	if (RAND() > .5)
	begin
		set @desctmp = @vlrtmp * (RAND() * 4 * 0.01);
		set @enctmp = @vlrtmp * (RAND() * 5 * 0.01);
	    set @vlrbxtmp = (@vlrtmp - @desctmp + @enctmp) * RAND() + @enctmp - @desctmp;
	end
	else
	begin
		set @desctmp = 0;
		set @enctmp = 0;
		set @vlrbxtmp = 0;
	end

     
	insert into Contas_a_Pagar (FK_Data_Emissao, 
		FK_Data_de_Vencimento, 
		FK_Forma_de_Pagamento, 
		FK_Empresa, 
		FK_Cliente,
		FK_Classificacao_Financeira,
		FK_Titulo_a_Pagar,
		VlrNomPg,
		VlrAbPg,
		VlrAtrasoPg,
		VlrDescPg,
		VlrEncPg,
		VlrPago)
		values (@dateemissao, 
				@datavenc,
				@formatmp,
				@empresatmp,
				@clientetmp,
				@ctmp3,
				@tittmp,
				@vlrtmp,
				@vlrtmp - @vlrbxtmp,
				0,
				@desctmp,
				@enctmp,
				@vlrbxtmp);
				
	set @cnt = @cnt + 1;
end	
commit transaction;

begin transaction;
set @cnt = 1;
while (@cnt < 15000)
begin
	set @tmp = CAST(Cast((RAND() * (2012 - 2004))+2004 AS INT) as varchar(4))
		+ '-' + CAST(Cast((RAND() * (12 - 1))+1 AS INT) as varchar(2))
		+ '-' + CAST(Cast((RAND() * (28 - 1))+1 AS INT) as varchar(2));
	set @datavenc =CAST(@tmp as datetime);
	
	set @tmp = CAST(Cast((RAND() * (2012 - 2004))+2004 AS INT) as varchar(4))
		+ '-' + CAST(Cast((RAND() * (12 - 1))+1 AS INT) as varchar(2))
		+ '-' + CAST(Cast((RAND() * (28 - 1))+1 AS INT) as varchar(2));
	set @dateemissao = CAST(@tmp as datetime);
	
	if (@datavenc < @dateemissao)
	begin
		set @tmpdate=@dateemissao;
		set @dateemissao=@datavenc;
		set @datavenc = @dateemissao;
	end

	set @cclas = Cast((RAND() * (@clascnt)) + 1 AS INT);
	with orderedclass as
    (
      select f.PK_ClassificacaoFinan, 
      ROW_NUMBER() over (order by PK_ClassificacaoFinan) as 'RowNumber'
      from ClassificaoFinanDim f
    )
	select @ctmp3 = oc.PK_ClassificacaoFinan
     from orderedclass oc
     where oc.RowNumber=@cclas;
     
     set @vlrtmp = ROUND( (6000 * RAND()) / (RAND() + 0.01), 2, 10);
     set @tittmp = 'CR00' + cast(Cast((RAND() * (@titulocnt)) + 1 AS INT) as varchar);
     
     set @formatmp= Cast((RAND() * (@formacnt)) + 1 AS INT);
	 set @empresatmp=Cast((RAND() * (@empresacnt)) + 1 AS INT);
	set @clientetmp=Cast((RAND() * (@clientecnt)) + 1 AS INT);
     
     	if (RAND() > .5)
	begin
		set @desctmp = @vlrtmp * (RAND() * 4 * 0.01);
		set @enctmp = @vlrtmp * (RAND() * 5 * 0.01);
	    set @vlrbxtmp = (@vlrtmp - @desctmp + @enctmp) * RAND() + @enctmp - @desctmp;
	end
	else
	begin
		set @desctmp = 0;
		set @enctmp = 0;
		set @vlrbxtmp = 0;
	end

     
	insert into Contas_a_Receber (FK_Data_Emissao, 
		FK_Data_de_Vencimento, 
		FK_Forma_de_Pagamento, 
		FK_Empresa, 
		FK_Cliente,
		FK_Classificacao_Financeira,
		FK_Titulo_a_Receber,
		VlrNomRec,
		VlrAbRec,
		VlrAtrasoRec,
		VlrDescRec,
		VlrEncRec,
		VlrRec)
		values (@dateemissao, 
				@datavenc,
				@formatmp,
				@empresatmp,
				@clientetmp,
				@ctmp3,
				@tittmp,
				@vlrtmp,
				@vlrtmp - @vlrbxtmp,
				0,
				@desctmp,
				@enctmp,
				@vlrbxtmp);
				
	set @cnt = @cnt + 1;
				
	set @cnt = @cnt + 1;
end	
commit transaction;

while (@@TRANCOUNT > 0)
begin
  commit transaction;
end
