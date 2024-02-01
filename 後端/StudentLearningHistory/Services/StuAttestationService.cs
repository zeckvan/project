using Dapper;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.StuAttestation.DbModels;
using StudentLearningHistory.Models.StuAttestation.Parameters;
using System.Data;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Reflection;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.StudCadre.DTOs;
using Microsoft.IdentityModel.Tokens;
using StudentLearningHistory.Models.StuCollege.Parameters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace StudentLearningHistory.Services
{
    public class StuAttestationService
    {
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        public string _schno;
        private string updteDate() => $"{DateTime.Now.Year - 1911}{DateTime.Now.ToString("MMddHHmmss")}";
        public StuAttestationService(IDapperContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _schno = _context.SchNo;
        }
        public async Task<IEnumerable<HeaderList>> GetCourseList(StuAttestationQueryList arg)
        {
            arg.sch_no = _context.SchNo;

            string str_sql = @"
with temp(sch_no,year_id,sms_id,cls_id,sub_id,src_dup,emp_id,std_no,ser_id,
	attestation_send,attestation_date,attestation_status,attestation_centraldb,
	content,is_sys,
	credit,grd_id,in_sms_id,borrow_yn,
	reread_yn,reread_yms,reread_type,repair_yn,repair_yms,repair_type,
	reread2_yn,reread2_yms,turn_yn,upyms,create_dt,yms,actually_year,actually_sms,sub_name,all_empname,stu_status,cls_name)
as
(
SELECT 
sch_no = s04_student.sch_no,
year_id = s04_stuhcls.year_id,
sms_id = s04_stuhcls.sms_id,
cls_id = 
convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
		convert(varchar(1),s04_noropenc.dep_id) + 
		case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
		convert(varchar(1),s04_noropenc.grd_id) + 
		case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
sub_id = convert(varchar(10),s04_subject.sub_id),
src_dup = convert(varchar(23),s04_stddbgo.course_code),
emp_id = convert(varchar(50),s04_noropenc.all_empid),
std_no = s04_student.std_no,
ser_id = 1,
attestation_send = '',
attestation_date = '',
attestation_status = '',
attestation_centraldb = 'N',
content = '',
is_sys = '2',
credit = convert(varchar(10),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
grd_id = convert(smallint,s04_stuhcls.grd_id),
in_sms_id = s04_stuhcls.sms_id,
borrow_yn = 'N',
reread_yn = 'N',
reread_yms = '0000',
reread_type = '0',
repair_yn  = 'N',
repair_yms = '0000',
repair_type = '0',
reread2_yn = 'N',
reread2_yms = '0000',
turn_yn = 'N',
upyms = '',
create_dt = '',
yms = '0000',
actually_year = s04_stuhcls.year_id,
actually_sms = s04_stuhcls.sms_id,
sub_name = s04_108subject.sub108_name,
all_empname = s04_noropenc.all_empname,
stu_status = '1',
cls_name = s04_noropenc.noroc_clsname
FROM s04_student
inner join s04_stuhcls on
	s04_student.std_no = s04_stuhcls.std_no and
	s04_stuhcls.year_id = @year_id and
	s04_stuhcls.sms_id = @sms_id and
	s04_student.std_no = @std_no
inner join s90_cha_id on
	s04_stuhcls.std_status = s90_cha_id.cha_id and
	s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
inner join s04_norstusc on
	s04_student.std_no = s04_norstusc.std_no and
	s04_norstusc.in_year = s04_stuhcls.year_id and
	s04_norstusc.in_sms = s04_stuhcls.sms_id and
	s04_norstusc.stu_status = 1 --新修
inner join s04_noropenc on
	s04_norstusc.noroc_id = s04_noropenc.noroc_id and
	isnull(s04_noropenc.course_code,'') <> '' and
	isnull(s04_noropenc.is_learn,'N') = 'Y'
inner join s04_subject on
	s04_noropenc.sub_id = s04_subject.sub_id
inner join s04_stddbgo on
	s04_stuhcls.year_id - s04_stuhcls.grd_id + 1 = s04_stddbgo.year_id and
	s04_stuhcls.sms_id = s04_stddbgo.sms_id and
	s04_stuhcls.deg_id = s04_stddbgo.deg_id and
	s04_stuhcls.dep_id = s04_stddbgo.dep_id and
	s04_stuhcls.bra_id = s04_stddbgo.bra_id and
	s04_stuhcls.grd_id = s04_stddbgo.grd_id and
	s04_stuhcls.cg_code = s04_stddbgo.cg_code and
	isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
	s04_noropenc.sub_id = s04_stddbgo.sub_id and
	( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
inner join s04_108subject on
	s04_stddbgo.course_code = s04_108subject.course_code
left join s90_organization on
	s04_noropenc.org_id = s90_organization.org_id
left join s90_branch on
	s04_noropenc.bra_id = s90_branch.bra_id
left join s04_hstusubjscoreterm on
	s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
	s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
	s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
	s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
	s04_hstusubjscoreterm.hstusst_status = 1
left join s04_hstusixscoreterm on
	s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
	s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
	s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
left join s04_stubuterm on
	s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
	s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
	s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
	s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
where s04_student.std_hisdatayear >= 108

union 

SELECT
sch_no = s04_student.sch_no,
year_id = s04_stuhcls.year_id,
sms_id = s04_stuhcls.sms_id,
cls_id = convert(varchar(6),s04_noropenc.noroc_id),
sub_id = convert(varchar(10),s04_subject.sub_id),
src_dup = convert(varchar(23),s04_stddbgo.course_code),
emp_id = convert(varchar(50),s04_noropenc.all_empid),
std_no = s04_student.std_no,
ser_id = 1,
attestation_send = '',
attestation_date = '',
attestation_status = '',
attestation_centraldb = 'N',
content = '',
is_sys = '2',
credit =  convert(varchar , s04_noropenc.sub_credit) ,
grd_id = convert(varchar(1) , noro_o.grd_id ),
in_sms_id = s04_stuhcls.sms_id,
borrow_yn = 'N',
reread_yn = 'Y',
reread_yms = convert(varchar,s04_stuhcls.year_id)+convert(varchar,s04_stuhcls.sms_id),
reread_type = '1',
repair_yn  = 'N',
repair_yms = '0000',
repair_type = '0',
reread2_yn = 'N',
reread2_yms = '0000',
turn_yn = 'N',
upyms = '',
create_dt = '',
yms = '0000',
actually_year = convert(smallint , s04_hstusubjscoreterm.in_year ),
actually_sms = convert(smallint , s04_hstusubjscoreterm.in_sms ),
sub_name = s04_108subject.sub108_name,
all_empname = s04_noropenc.all_empname,
stu_status = '2',
cls_name = s04_noropenc.noroc_clsname
FROM s04_student
inner join s04_stuhcls on
	s04_student.std_no = s04_stuhcls.std_no and
	s04_stuhcls.year_id = @year_id and
	s04_stuhcls.sms_id = @sms_id and
	s04_student.std_no = @std_no
inner join s90_cha_id on
	s04_stuhcls.std_status = s90_cha_id.cha_id and
	s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370') --只上傳正常學籍、休學、輔導延修
inner join s04_noropenc on
	s04_noropenc.year_id = s04_stuhcls.year_id and
	s04_noropenc.sms_id = s04_stuhcls.sms_id
inner join s04_norstusc on
	s04_norstusc.noroc_id = s04_noropenc.noroc_id and
	s04_student.std_no = s04_norstusc.std_no and
	s04_norstusc.stu_status = 2 --重修
inner join s04_subject on
	s04_noropenc.sub_id = s04_subject.sub_id
inner join s04_hstusubjscoreterm on
	s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
	s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
	s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
	s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
	s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
	s04_hstusubjscoreterm.hstusst_status = 2
inner join s04_stddbgo on
	s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
	s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
	s04_stuhcls.deg_id = s04_stddbgo.deg_id and
	s04_stuhcls.dep_id = s04_stddbgo.dep_id and
	s04_stuhcls.bra_id = s04_stddbgo.bra_id and
	s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
	s04_stuhcls.cg_code = s04_stddbgo.cg_code and
	isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
	s04_noropenc.sub_id = s04_stddbgo.sub_id and
	( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
inner join s04_108subject on
	s04_stddbgo.course_code = s04_108subject.course_code
inner join s90_organization on
	s04_noropenc.org_id = s90_organization.org_id
inner join s90_branch on
	s04_noropenc.bra_id = s90_branch.bra_id
inner join s04_hstusixscoreterm on
	s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
	s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
	s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
left join s04_stubuterm on
	s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
	s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
	s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
	s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id 
inner join s04_hstusubjscoreterm hstusst_new on
	s04_hstusubjscoreterm.sub_id = hstusst_new.sub_id and
	s04_hstusubjscoreterm.std_no = hstusst_new.std_no and
	s04_hstusubjscoreterm.in_year = hstusst_new.in_year and
	s04_hstusubjscoreterm.in_sms = hstusst_new.in_sms and
	hstusst_new.hstusst_status = 1
inner join s04_norstusc norstu_o on
	hstusst_new.std_no = norstu_o.std_no and
	hstusst_new.in_year = norstu_o.in_year and
	hstusst_new.in_sms = norstu_o.in_sms and
	norstu_o.stu_status = 1
inner join s04_noropenc noro_o on
	norstu_o.noroc_id = noro_o.noroc_id and
	hstusst_new.sub_id = noro_o.sub_id 		
inner join s90_organization org_o on
	noro_o.org_id = org_o.org_id
where s04_student.std_hisdatayear >= 108 

union

SELECT
sch_no = s04_student.sch_no,
year_id = s04_stuhcls.year_id,
sms_id = s04_stuhcls.sms_id,
cls_id = convert(varchar(6),s04_noropenc.noroc_id),
sub_id = convert(varchar(10),s04_subject.sub_id),
src_dup = convert(varchar(23),s04_stddbgo.course_code),
emp_id = convert(varchar(50),s04_noropenc.all_empid),
std_no = s04_student.std_no,
ser_id = 1,
attestation_send = '',
attestation_date = '',
attestation_status = '',
attestation_centraldb = 'N',
content = '',
is_sys = '2',
credit = convert(varchar , s04_stddbgo.sub_credit) ,
grd_id = convert(smallint,s04_stuhcls.grd_id),
in_sms_id = s04_stuhcls.sms_id,
borrow_yn = 'N',
reread_yn = 'N',
reread_yms = '0000',
reread_type = '0',
repair_yn  = 'Y',
repair_yms = convert(varchar , s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 )+convert(varchar , s04_stddbgo.sms_id ),
repair_type = '1',
reread2_yn = 'N',
reread2_yms = '0000',
turn_yn = 'N',
upyms = '',
create_dt = '',
yms = '0000',
actually_year = convert(smallint , s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 ),
actually_sms =  convert(smallint , s04_stddbgo.sms_id ),
sub_name = subj108_o.sub108_name,
all_empname = s04_noropenc.all_empname,
stu_status = '3',
cls_name = s04_noropenc.noroc_clsname
FROM s04_student
inner join s04_stuhcls on
	s04_student.std_no = s04_stuhcls.std_no and
	s04_stuhcls.year_id = @year_id and
	s04_stuhcls.sms_id = @sms_id and
	s04_student.std_no = @std_no
inner join s90_cha_id on
	s04_stuhcls.std_status = s90_cha_id.cha_id and
	s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
inner join s04_noropenc on
	s04_noropenc.year_id = s04_stuhcls.year_id and
	s04_noropenc.sms_id = s04_stuhcls.sms_id 
inner join s04_norstusc on
	s04_norstusc.noroc_id = s04_noropenc.noroc_id and
	s04_student.std_no = s04_norstusc.std_no and
	s04_norstusc.stu_status = 3 --補修
inner join s04_subject on
	s04_noropenc.sub_id = s04_subject.sub_id
inner join s90_organization on
	s04_noropenc.org_id = s90_organization.org_id
inner join s04_hstusubjscoreterm on
	s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
	s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
	s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
	s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
	s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
	s04_hstusubjscoreterm.hstusst_status = 3
left join s04_hstusixscoreterm on
	s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
	s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
	s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
left join s04_stubuterm on
	s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
	s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
	s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
	s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
inner join s04_stddbgo on
	s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
	s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
	s04_stuhcls.deg_id = s04_stddbgo.deg_id and
	s04_stuhcls.dep_id = s04_stddbgo.dep_id and
	s04_stuhcls.bra_id = s04_stddbgo.bra_id and
	s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
	s04_stuhcls.cg_code = s04_stddbgo.cg_code and
	isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
	s04_noropenc.sub_id = s04_stddbgo.sub_id and
	( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
inner join s04_108subject on
	s04_stddbgo.course_code = s04_108subject.course_code
inner join s04_ytdbgoc on
	s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 = s04_ytdbgoc.year_id and
	s04_stddbgo.sms_id = s04_ytdbgoc.sms_id and
	s04_stddbgo.dep_id = s04_ytdbgoc.dep_id and
	s04_stddbgo.deg_id = s04_ytdbgoc.deg_id and
	s04_stddbgo.bra_id = s04_ytdbgoc.bra_id and
	s04_stddbgo.grd_id = s04_ytdbgoc.grd_id and
	s04_stuhcls.cls_id = s04_ytdbgoc.cls_id
inner join s90_organization org_o on
	s04_ytdbgoc.org_id = org_o.org_id
inner join s90_branch on
	s04_ytdbgoc.bra_id = s90_branch.bra_id
inner join s04_108subject subj108_o on
	s04_stddbgo.course_code = subj108_o.course_code
where s04_student.std_hisdatayear >= 108
)
SELECT 
*,
x_cnt2 = 
	(select count(*)
		from L01_std_public_filehub a
		where a.class_name = 'StuAttestation'
		and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0'),
x_cnt = 
	(select count(*)
		from L01_std_public_filehub a
		where a.class_name = 'StuAttestation'
		and  a.number_id = NewTable.i
		and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0'),
x_total =
(select count(*)
from L01_std_attestation a
where a.year_id = NewTable.b
and   a.sms_id = NewTable.c
and   a.std_no = NewTable.h),
x_file_cnt =
			(select value
				from L01_setup
				where id = 2) 
FROM(
select *,
		ROW_NUMBER() OVER(ORDER BY a.b,a.c,a.d,a.e,a.f) AS RowNum
from (
			select 
			a = temp.sch_no,
			b = temp.year_id,
			c = temp.sms_id ,
			d = temp.cls_id,
			e = temp.sub_id,
			f = temp.src_dup,
			g = temp.emp_id,
			h = temp.std_no,
			i =0,
			j = temp.credit,
			k = '',
			l ='',
			m = '',
			n = '',
			o = '',
			p = '',
			q = temp.sub_name,
			r = temp.all_empname,
			s = '',		
			w = '',
			x = ''
			from temp
			where not exists(
							select 1
							from L01_std_attestation
							where temp.sch_no = L01_std_attestation.sch_no 
							and temp.year_id = L01_std_attestation.year_id 
							and temp.sms_id = L01_std_attestation.sms_id 
							and temp.cls_id = L01_std_attestation.cls_id 
							and temp.sub_id = L01_std_attestation.sub_id 
							and temp.src_dup = L01_std_attestation.src_dup 
							and temp.emp_id = L01_std_attestation.emp_id 
							and temp.std_no = L01_std_attestation.std_no)
			and sch_no = @sch_no  
			and year_id = @year_id  
			and sms_id = @sms_id 
			and std_no = @std_no
			
			union 

			select 
			a = sch_no,
			b = year_id,
			c = sms_id ,
			d = cls_id,
			e = sub_id,
			f = src_dup,
			g = emp_id,
			h = std_no,
			i = L01_std_attestation.ser_id,
			j = credit,
			k = '',
			l = case 
				when isnull(L01_std_attestation.attestation_send,'') = '' then '' 
				else	substring(L01_std_attestation.attestation_send,1,3)+'/'+
						substring(L01_std_attestation.attestation_send,4,2)+'/'+
						substring(L01_std_attestation.attestation_send,6,2)+' '+
						substring(L01_std_attestation.attestation_send,8,2)+':'+
						substring(L01_std_attestation.attestation_send,10,2)+':'+
						substring(L01_std_attestation.attestation_send,12,2)
				end,
			m = case
				when isnull(attestation_release,'') = '' then '' 
				else	substring(L01_std_attestation.attestation_date,1,3)+'/'+
						substring(L01_std_attestation.attestation_date,4,2)+'/'+
						substring(L01_std_attestation.attestation_date,6,2)+' '+
						substring(L01_std_attestation.attestation_date,8,2)+':'+
						substring(L01_std_attestation.attestation_date,10,2)+':'+
						substring(L01_std_attestation.attestation_date,12,2)
				end,
			n = case when isnull(attestation_release,'') = '' and isnull(L01_std_attestation.attestation_status,'') = '' then '' else L01_std_attestation.attestation_status end,
			o = L01_std_attestation.is_sys,
			p = L01_std_attestation.content,
			q = sub_name,
			r = all_empname,
			s = 
			(
			Select SUBSTRING(
				(Select ','+ file_name
					From L01_std_public_filehub b
					where 
					b.complex_key = L01_std_attestation.sch_no+'_'+
												convert(varchar,L01_std_attestation.year_id)+'_'+
												convert(varchar,L01_std_attestation.sms_id)+'_'+
												L01_std_attestation.cls_id+'_'+
												L01_std_attestation.sub_id+'_'+
												L01_std_attestation.src_dup+'_'+
												L01_std_attestation.emp_id+'_'+
												L01_std_attestation.std_no+'_0' and
					b.class_name = 'StuAttestation'  and
				    b.number_id = L01_std_attestation.ser_id
				For Xml Path(''))
			, 2, 8000)
			),
			w = case isnull(attestation_reason,'')
				when '01' then '非本課程相關主題成果，無法認證'
				when '02' then '無法查證成果可信度'
				when '03' then '請加強成果內容正確性'
				when '04' then '請增加成果內容豐富度'  else '' end,
			x = attestation_release
			from L01_std_attestation
			where sch_no = @sch_no  
			and year_id = @year_id  
			and sms_id = @sms_id 
			and std_no = @std_no
			)a
)as NewTable
order by
d,e";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<HeaderList>(str_sql,arg);
            }
        }
        public async Task<IEnumerable<HeaderList>> GetList(StuAttestationQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            #region old sql
            //    string str_sql = @"
            //                        SELECT *,
            //                            (select count(*)
            //                            from L01_std_public_filehub a
            //                            where a.class_name = 'StuAttestation'
            //                            and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0') as x_cnt,
            //	x_total =
            //	(select count(*)
            //	 from s04_norstusc a,s04_noropenc b
            //	  where	convert(varchar,NewTable.b)= a.in_year and 
            //			convert(varchar,NewTable.c) = a.in_sms and 
            //			NewTable.as_std_no = a.std_no and 
            //			a.noroc_id = b.noroc_id and
            //			a.stu_status in(1) and
            //			isnull(b.course_code,'') <> '' and
            //			isnull(b.is_learn,'N') = 'Y')
            //	+
            //	(select count(*)
            //	 from s04_norstusc a,s04_noropenc b
            //	  where NewTable.as_std_no = a.std_no and 
            //			a.noroc_id = b.noroc_id and
            //			a.stu_status in(2,3)),
            //x_file_cnt =
            //						(select  value
            //						from L01_setup
            //						where id = 2)
            //                        FROM(
            //		select *,
            //				ROW_NUMBER() OVER(ORDER BY a.b,a.c,a.as_cls_uid,a.as_subj_uid) AS RowNum
            //		from (
            //		SELECT 
            //				as_id_no = convert(varchar(10),s04_student.std_identity) ,		--身份證字號
            //				as_std_no = convert(varchar(10),s04_student.std_no),			--學號
            //				as_grade = convert(smallint,s04_stuhcls.grd_id) ,				--學生年級
            //				as_cls_uid = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
            //					convert(varchar(1),s04_noropenc.dep_id) + 
            //					case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
            //					convert(varchar(1),s04_noropenc.grd_id) + 
            //					case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
            //				 as_cls_no = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級代碼
            //					convert(varchar(1),s04_noropenc.dep_id) + 
            //					case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
            //					convert(varchar(1),s04_noropenc.grd_id) + 
            //					case when s04_noropenc.cls_id >= 10 then '0' + convert(varchar(2),s04_noropenc.cls_id) else '00' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
            //				as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname), --開課班級名稱
            //				as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end), --開課班級內分組
            //				as_subj_uid = convert(varchar(10),s04_subject.sub_id), --科目內碼
            //				as_course_id = convert(varchar(23),s04_stddbgo.course_code), --課程計畫平臺核發之課程代碼	
            //				as_subj_name = convert(varchar(90),s04_108subject.sub108_name), --科目名稱
            //				as_credits = convert(varchar(10),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end), --學分/時數
            //				as_type = convert(varchar(1),s04_norstusc.stu_status),	--修課類別
            //				as_use_credits = convert(varchar(1),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ), --是否採計學分
            //				as_o_syear =null, --原(應)修課學年度
            //				as_o_sems = null, --原(應)修課學期
            //				as_o_cls_year = null, --原(應)開課年級
            //				as_o_cls_no = null, --原(應)開課班級代碼
            //				as_o_cls_name = null, --原(應)開課班級名稱
            //				as_o_cls_group = null, --原(應)開課班級內分組
            //				as_o_subj_id = null, --原(應)修校務系統科目內碼
            //				as_o_course_id = null , --原(應)修課程代碼
            //				as_o_subj_name = null, --原(應)修科目名稱
            //				as_o_credits = null, --原(應)修學分
            //				as_makeup_mode = null, --補修方式
            //				as_rebuild_mode = null, --重修方式
            //				as_is_precourse = '0', --是否為預選課程
            //				as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid), --授課教師內碼
            //				as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode), --授課教師代碼
            //							as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname), --授課教師姓名
            //				s04_stuhcls.sch_no as a,
            //				s04_stuhcls.year_id as b,
            //				s04_stuhcls.sms_id as c,
            //				d = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
            //					convert(varchar(1),s04_noropenc.dep_id) + 
            //					case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
            //					convert(varchar(1),s04_noropenc.grd_id) + 
            //					case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
            //				e = convert(varchar(10),s04_subject.sub_id),
            //				f = convert(varchar(23),s04_stddbgo.course_code),
            //				g = convert(varchar(50),s04_noropenc.all_empid),
            //				h = s04_student.std_no ,
            //				a.ser_id as i,
            //				s04_noropenc.sub_credit as j,
            //				convert(varchar(1),s04_norstusc.stu_status) as k,
            //				case when isnull(a.attestation_send,'') = '' then '' 
            //				else substring(a.attestation_send,1,3)+'/'+
            //				substring(a.attestation_send,4,2)+'/'+
            //				substring(a.attestation_send,6,2)+' '+
            //				substring(a.attestation_send,8,2)+':'+
            //				substring(a.attestation_send,10,2)+':'+
            //				substring(a.attestation_send,12,2)
            //				end  as l,
            //				case when isnull(a.attestation_release,'') = '' then '' 
            //				else substring(a.attestation_date,1,3)+'/'+
            //				substring(a.attestation_date,4,2)+'/'+
            //				substring(a.attestation_date,6,2)+' '+
            //				substring(a.attestation_date,8,2)+':'+
            //				substring(a.attestation_date,10,2)+':'+
            //				substring(a.attestation_date,12,2)
            //				end  as m,
            //				case when isnull(a.attestation_release,'') = '' and isnull(a.attestation_status,'') = '' then '' else a.attestation_status end as n,
            //				a.is_sys as o,
            //				a.content as p,
            //				convert(varchar(90),s04_108subject.sub108_name) as q,
            //				convert(varchar(50),s04_noropenc.all_empname) as r,
            //				s = 
            //				(
            //				Select SUBSTRING(
            //					(Select ','+ file_name
            //					From L01_std_public_filehub b
            //				where 
            //						b.complex_key = s04_stuhcls.sch_no+'_'+
            //										convert(varchar,s04_stuhcls.year_id)+'_'+
            //										convert(varchar,s04_stuhcls.sms_id)+'_'+
            //										convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
            //										convert(varchar(1),s04_noropenc.dep_id) + 
            //										case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
            //										convert(varchar(1),s04_noropenc.grd_id) + 
            //										case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end )+'_'+
            //											convert(varchar(10),s04_subject.sub_id)+'_'+
            //										convert(varchar(23),s04_stddbgo.course_code)+'_'+
            //										convert(varchar(50),s04_noropenc.all_empid)+'_'+
            //										s04_student.std_no+'_0' and
            //						b.class_name = 'StuAttestation' 
            //					For Xml Path(''))
            //				, 2, 8000)
            //				),
            //				w = case isnull(a.attestation_reason,'') when '01' then '非本課程相關主題成果，無法認證'
            //																		when '02' then '無法查證成果可信度'
            //																		when '03' then '請加強成果內容正確性'
            //																		when '04' then '請增加成果內容豐富度'  else '' end,
            //				x = a.attestation_release
            //			FROM s04_student
            //			inner join s04_stuhcls on
            //				s04_student.std_no = s04_stuhcls.std_no and												
            //				s04_stuhcls.sch_no = @sch_no and 
            //				s04_stuhcls.year_id = @year_id and 
            //				s04_stuhcls.sms_id = @sms_id and
            //				s04_student.std_no = @std_no
            //			inner join s90_cha_id on
            //				s04_stuhcls.std_status = s90_cha_id.cha_id and
            //				s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
            //			inner join s04_norstusc on
            //				s04_student.std_no = s04_norstusc.std_no and
            //				s04_norstusc.in_year = s04_stuhcls.year_id and
            //				s04_norstusc.in_sms = s04_stuhcls.sms_id and
            //				s04_norstusc.stu_status = 1 --新修
            //			inner join s04_noropenc on
            //				s04_norstusc.noroc_id = s04_noropenc.noroc_id and
            //				isnull(s04_noropenc.course_code,'') <> '' and
            //				isnull(s04_noropenc.is_learn,'N') = 'Y'
            //			inner join s04_subject on
            //				s04_noropenc.sub_id = s04_subject.sub_id
            //			inner join s04_stddbgo on
            //				s04_stuhcls.year_id - s04_stuhcls.grd_id + 1 = s04_stddbgo.year_id and
            //				s04_stuhcls.sms_id = s04_stddbgo.sms_id and
            //				s04_stuhcls.deg_id = s04_stddbgo.deg_id and
            //				s04_stuhcls.dep_id = s04_stddbgo.dep_id and
            //				s04_stuhcls.bra_id = s04_stddbgo.bra_id and
            //				s04_stuhcls.grd_id = s04_stddbgo.grd_id and
            //				s04_stuhcls.cg_code = s04_stddbgo.cg_code and
            //				isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
            //				s04_noropenc.sub_id = s04_stddbgo.sub_id and
            //				( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
            //			inner join s04_108subject on
            //				s04_stddbgo.course_code = s04_108subject.course_code
            //			left join s90_organization on
            //				s04_noropenc.org_id = s90_organization.org_id
            //			left join s90_branch on
            //				s04_noropenc.bra_id = s90_branch.bra_id
            //			left join s04_hstusubjscoreterm on
            //				s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
            //				s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
            //				s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
            //				s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
            //				s04_hstusubjscoreterm.hstusst_status = 1
            //			left join s04_hstusixscoreterm on
            //				s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
            //				s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
            //				s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
            //			left join s04_stubuterm on
            //				s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
            //				s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
            //				s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
            //				s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
            //			left join L01_std_attestation a on
            //				a.year_id = s04_stuhcls.year_id and
            //				a.sms_id = s04_stuhcls.sms_id and
            //				a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
            //						convert(varchar(1),s04_noropenc.dep_id) + 
            //						case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
            //						convert(varchar(1),s04_noropenc.grd_id) + 
            //						case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ) and
            //				a.sub_id = convert(varchar(10),s04_subject.sub_id) and
            //				a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
            //				a.emp_id =  convert(varchar(50),s04_noropenc.all_empid) and
            //				a.std_no = s04_student.std_no
            //			where s04_student.std_hisdatayear >= 108
            //		union
            //		SELECT 
            //				as_id_no = convert(varchar(10),s04_student.std_identity) ,
            //				as_std_no = convert(varchar(10),s04_student.std_no),
            //				as_grade = convert(smallint,s04_stuhcls.grd_id) ,
            //				as_cls_uid = convert(varchar(6),s04_noropenc.noroc_id),
            //				as_cls_no = convert(varchar(6),s04_noropenc.noroc_id),
            //				as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname),
            //				as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end),
            //				as_subj_uid = convert(varchar(10),s04_subject.sub_id),
            //				as_course_id = convert(varchar(23),s04_stddbgo.course_code),
            //				as_subj_name = convert(varchar(90),s04_108subject.sub108_name),
            //				as_credits = convert(varchar(1),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
            //				as_type = convert(varchar(1),s04_norstusc.stu_status),
            //				as_use_credits = convert(varchar(10),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ),
            //				as_o_syear = convert(smallint , s04_hstusubjscoreterm.in_year ),
            //				as_o_seme = convert(smallint , s04_hstusubjscoreterm.in_sms ),
            //				as_o_cls_year = convert(varchar(1) , noro_o.grd_id ),
            //				as_o_cls_no = convert(varchar(1),noro_o.dep_id) + 
            //					case when noro_o.bra_id >= 10 then convert(varchar(2),noro_o.bra_id) else '0' + convert(varchar(1),noro_o.bra_id) end + 
            //					convert(varchar(1),noro_o.grd_id) + 
            //					case when noro_o.cls_id >= 100 then convert(varchar(3),noro_o.cls_id) when noro_o.cls_id >= 10 then '0' + convert(varchar(2),noro_o.cls_id) else '00' + convert(varchar(1),noro_o.cls_id) end,
            //				 as_o_cls_name = convert(varchar(90),noro_o.noroc_clsname),
            //				as_o_cls_group = convert(varchar(20),case when isnull(noro_o.org_id,0) in (0,99) then '無' else org_o.org_name end) ,
            //				as_o_subj_id = convert(varchar(10),noro_o.sub_id),
            //				as_o_course_id = convert(varchar(23),noro_o.course_code),
            //				as_o_subj_name = convert(varchar(90),s04_108subject.sub108_name),
            //				as_o_credits = convert(smallint , s04_noropenc.sub_credit) ,
            //				as_makeup_mode = null,
            //				as_rebuild_mode = convert(varchar(1),case isnull(s04_hstusubjscoreterm.hstusst_outsch,'0') when '1' then 4 when '2' then 5 when '3' then 6 end),
            //				as_is_precourse = '0',
            //				as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid),
            //				as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode),
            //							as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
            //				s04_stuhcls.sch_no as a,
            //				s04_stuhcls.year_id as b,
            //				s04_stuhcls.sms_id as c,
            //				d = convert(varchar(6),s04_noropenc.noroc_id),
            //				e = convert(varchar(10),s04_subject.sub_id),
            //				f = convert(varchar(23),s04_stddbgo.course_code),
            //				g = convert(varchar(50),s04_noropenc.all_empid),
            //				h = s04_student.std_no ,
            //				a.ser_id as i,
            //				s04_noropenc.sub_credit as j,
            //				convert(varchar(1),s04_norstusc.stu_status) as k,
            //				case when isnull(a.attestation_send,'') = '' then '' 
            //				else substring(a.attestation_send,1,3)+'/'+
            //				substring(a.attestation_send,4,2)+'/'+
            //				substring(a.attestation_send,6,2)+' '+
            //				substring(a.attestation_send,8,2)+':'+
            //				substring(a.attestation_send,10,2)+':'+
            //				substring(a.attestation_send,12,2)
            //				end  as l,
            //				case when isnull(a.attestation_release,'') = '' then '' 
            //				else substring(a.attestation_date,1,3)+'/'+
            //				substring(a.attestation_date,4,2)+'/'+
            //				substring(a.attestation_date,6,2)+' '+
            //				substring(a.attestation_date,8,2)+':'+
            //				substring(a.attestation_date,10,2)+':'+
            //				substring(a.attestation_date,12,2)
            //				end  as m,
            //				case when isnull(a.attestation_release,'') = '' and isnull(a.attestation_status,'') = '' then '' else a.attestation_status end as n,
            //				a.is_sys as o,
            //				a.content as p,
            //				convert(varchar(90),s04_108subject.sub108_name) as q,
            //				convert(varchar(50),s04_noropenc.all_empname) as r,
            //				s = 
            //				(
            //				Select SUBSTRING(
            //					(Select ','+ file_name
            //					From L01_std_public_filehub b
            //				where 
            //						b.complex_key = s04_stuhcls.sch_no+'_'+
            //										convert(varchar,s04_stuhcls.year_id)+'_'+
            //										convert(varchar,s04_stuhcls.sms_id)+'_'+
            //										convert(varchar(6),s04_noropenc.noroc_id)+'_'+
            //										convert(varchar(10),s04_subject.sub_id)+'_'+
            //										convert(varchar(23),s04_stddbgo.course_code)+'_'+
            //										convert(varchar(50),s04_noropenc.all_empid)+'_'+
            //										s04_student.std_no+'_0' and
            //						b.class_name = 'StuAttestation' 
            //					For Xml Path(''))
            //				, 2, 8000)
            //				),
            //				w = case isnull(a.attestation_reason,'') when '01' then '非本課程相關主題成果，無法認證'
            //																		when '02' then '無法查證成果可信度'
            //																		when '03' then '請加強成果內容正確性'
            //																		when '04' then '請增加成果內容豐富度'  else '' end,
            //				x = a.attestation_release
            //			FROM s04_student
            //			inner join s04_stuhcls on
            //				s04_student.std_no = s04_stuhcls.std_no and												
            //				s04_stuhcls.sch_no = @sch_no and 
            //				s04_stuhcls.year_id = @year_id and 
            //				s04_stuhcls.sms_id = @sms_id and
            //				s04_student.std_no = @std_no
            //			inner join s90_cha_id on
            //				s04_stuhcls.std_status = s90_cha_id.cha_id and
            //				s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370') --只上傳正常學籍、休學、輔導延修
            //			inner join s04_noropenc on
            //				s04_noropenc.year_id = s04_stuhcls.year_id and
            //				s04_noropenc.sms_id = s04_stuhcls.sms_id
            //			inner join s04_norstusc on
            //				s04_norstusc.noroc_id = s04_noropenc.noroc_id and
            //				s04_student.std_no = s04_norstusc.std_no and
            //				s04_norstusc.stu_status = 2 --重修
            //			inner join s04_subject on
            //				s04_noropenc.sub_id = s04_subject.sub_id
            //			inner join s04_hstusubjscoreterm on
            //				s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
            //				s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
            //				s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
            //				s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
            //				s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
            //				s04_hstusubjscoreterm.hstusst_status = 2
            //			inner join s04_stddbgo on
            //				s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
            //				s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
            //				s04_stuhcls.deg_id = s04_stddbgo.deg_id and
            //				s04_stuhcls.dep_id = s04_stddbgo.dep_id and
            //				s04_stuhcls.bra_id = s04_stddbgo.bra_id and
            //				s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
            //				s04_stuhcls.cg_code = s04_stddbgo.cg_code and
            //				isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
            //				s04_noropenc.sub_id = s04_stddbgo.sub_id and
            //				( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
            //			inner join s04_108subject on
            //				s04_stddbgo.course_code = s04_108subject.course_code
            //			inner join s90_organization on
            //				s04_noropenc.org_id = s90_organization.org_id
            //			inner join s90_branch on
            //				s04_noropenc.bra_id = s90_branch.bra_id
            //			inner join s04_hstusixscoreterm on
            //				s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
            //				s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
            //				s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
            //			left join s04_stubuterm on
            //				s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
            //				s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
            //				s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
            //				s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id 
            //			inner join s04_hstusubjscoreterm hstusst_new on
            //				s04_hstusubjscoreterm.sub_id = hstusst_new.sub_id and
            //				s04_hstusubjscoreterm.std_no = hstusst_new.std_no and
            //				s04_hstusubjscoreterm.in_year = hstusst_new.in_year and
            //				s04_hstusubjscoreterm.in_sms = hstusst_new.in_sms and
            //				hstusst_new.hstusst_status = 1
            //			inner join s04_norstusc norstu_o on
            //				hstusst_new.std_no = norstu_o.std_no and
            //				hstusst_new.in_year = norstu_o.in_year and
            //				hstusst_new.in_sms = norstu_o.in_sms and
            //				norstu_o.stu_status = 1
            //			inner join s04_noropenc noro_o on
            //				norstu_o.noroc_id = noro_o.noroc_id and
            //				hstusst_new.sub_id = noro_o.sub_id 		
            //			inner join s90_organization org_o on
            //				noro_o.org_id = org_o.org_id
            //			left join L01_std_attestation a on
            //				a.year_id = s04_stuhcls.year_id and
            //				a.sms_id = s04_stuhcls.sms_id and
            //				a.cls_id = convert(varchar(6),s04_noropenc.noroc_id) and
            //				a.sub_id = convert(varchar(10),s04_subject.sub_id) and
            //				a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
            //				a.emp_id =  convert(varchar(50),s04_noropenc.all_empid)and
            //				a.std_no =s04_student.std_no
            //			where s04_student.std_hisdatayear >= 108 
            //		union
            //		SELECT 
            //				as_id_no = convert(varchar(10),s04_student.std_identity) ,
            //				as_std_no = convert(varchar(10),s04_student.std_no),
            //				as_grade = convert(smallint,s04_stuhcls.grd_id) ,
            //				as_cls_uid = convert(varchar(6),s04_noropenc.noroc_id),
            //				as_cls_no = convert(varchar(6),s04_noropenc.noroc_id),
            //				as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname),
            //				as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end),
            //				as_subj_uid = convert(varchar(10),s04_subject.sub_id),
            //				as_course_id = convert(varchar(23),s04_stddbgo.course_code),
            //				as_subj_name = convert(varchar(90),s04_108subject.sub108_name),
            //				as_credits = convert(varchar(1),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
            //				as_type = convert(varchar(1),s04_norstusc.stu_status),
            //				as_use_credits = convert(varchar(10),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ),
            //				as_o_syear = convert(smallint , s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 ),
            //				as_o_sems = convert(smallint , s04_stddbgo.sms_id ),
            //				as_o_cls_year = convert(varchar(1) , s04_stddbgo.grd_id ),
            //				as_o_cls_no = convert(varchar(1),s04_stddbgo.dep_id) + 
            //					case when s04_stddbgo.bra_id >= 10 then convert(varchar(2),s04_stddbgo.bra_id) else '0' + convert(varchar(1),s04_stddbgo.bra_id) end + 
            //					convert(varchar(1),s04_stddbgo.grd_id) + 
            //					case when s04_stuhcls.cls_id >= 10 then '0' + convert(varchar(2),s04_stuhcls.cls_id) else '00' + convert(varchar(1),s04_stuhcls.cls_id) end,
            //				as_o_cls_name = convert(varchar(60),s04_ytdbgoc.cls_abr),
            //				as_o_cls_group = convert(varchar(20),case when isnull(s04_ytdbgoc.org_id,0) in (0,99) then '無' else org_o.org_name end) ,
            //				as_o_subj_id = convert(varchar(10),s04_stddbgo.sub_id),
            //				as_o_course_id = convert(varchar(23),s04_stddbgo.course_code),
            //				as_o_subj_name = convert(varchar(90),subj108_o.sub108_name),
            //				as_o_credits = convert(smallint , s04_stddbgo.sub_credit) ,
            //				as_makeup_mode = convert(varchar(1),case isnull(s04_hstusubjscoreterm.hstusst_outsch,'0') when '1' then 4 when '2' then 5 when '3' then 6 end),
            //				as_rebuild_mode = null,
            //				as_is_precourse = '0',
            //				as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid),
            //				as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode),
            //							as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
            //				s04_stuhcls.sch_no as a,
            //				s04_stuhcls.year_id as b,
            //				s04_stuhcls.sms_id as c,
            //				d = convert(varchar(6),s04_noropenc.noroc_id),
            //				e = convert(varchar(10),s04_subject.sub_id),
            //				f = convert(varchar(23),s04_stddbgo.course_code),
            //				g = convert(varchar(50),s04_noropenc.all_empid),
            //				h = s04_student.std_no ,
            //				a.ser_id as i,
            //				s04_noropenc.sub_credit as j,
            //				convert(varchar(1),s04_norstusc.stu_status) as k,
            //				case when isnull(a.attestation_send,'') = '' then '' 
            //				else substring(a.attestation_send,1,3)+'/'+
            //				substring(a.attestation_send,4,2)+'/'+
            //				substring(a.attestation_send,6,2)+' '+
            //				substring(a.attestation_send,8,2)+':'+
            //				substring(a.attestation_send,10,2)+':'+
            //				substring(a.attestation_send,12,2)
            //				end  as l,
            //				case when isnull(a.attestation_release,'') = '' then '' 
            //				else substring(a.attestation_date,1,3)+'/'+
            //				substring(a.attestation_date,4,2)+'/'+
            //				substring(a.attestation_date,6,2)+' '+
            //				substring(a.attestation_date,8,2)+':'+
            //				substring(a.attestation_date,10,2)+':'+
            //				substring(a.attestation_date,12,2)
            //				end  as m,
            //				case when isnull(a.attestation_release,'') = '' and isnull(a.attestation_status,'') = '' then '' else a.attestation_status end as n,
            //				a.is_sys as o,
            //				a.content as p,
            //				convert(varchar(90),s04_108subject.sub108_name) as q,
            //				convert(varchar(50),s04_noropenc.all_empname) as r,
            //				s = 
            //				(
            //				Select SUBSTRING(
            //					(Select ','+ file_name
            //					From L01_std_public_filehub b
            //				where 
            //						b.complex_key = s04_stuhcls.sch_no+'_'+
            //										convert(varchar,s04_stuhcls.year_id)+'_'+
            //										convert(varchar,s04_stuhcls.sms_id)+'_'+
            //										convert(varchar(6),s04_noropenc.noroc_id)+'_'+
            //										convert(varchar(10),s04_subject.sub_id)+'_'+
            //										convert(varchar(23),s04_stddbgo.course_code)+'_'+
            //										convert(varchar(50),s04_noropenc.all_empid)+'_'+
            //										s04_student.std_no+'_0' and
            //						b.class_name = 'StuAttestation' 
            //					For Xml Path(''))
            //				, 2, 8000)
            //				),
            //				w = case isnull(a.attestation_reason,'') when '01' then '非本課程相關主題成果，無法認證'
            //																		when '02' then '無法查證成果可信度'
            //																		when '03' then '請加強成果內容正確性'
            //																		when '04' then '請增加成果內容豐富度'  else '' end,
            //				x = a.attestation_release
            //			FROM s04_student
            //			inner join s04_stuhcls on
            //				s04_student.std_no = s04_stuhcls.std_no and												
            //				s04_stuhcls.sch_no = @sch_no and 
            //				s04_stuhcls.year_id = @year_id and 
            //				s04_stuhcls.sms_id = @sms_id and
            //				s04_student.std_no = @std_no
            //			inner join s90_cha_id on
            //				s04_stuhcls.std_status = s90_cha_id.cha_id and
            //				s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
            //			inner join s04_noropenc on
            //				s04_noropenc.year_id = s04_stuhcls.year_id and
            //				s04_noropenc.sms_id = s04_stuhcls.sms_id 
            //			inner join s04_norstusc on
            //				s04_norstusc.noroc_id = s04_noropenc.noroc_id and
            //				s04_student.std_no = s04_norstusc.std_no and
            //				s04_norstusc.stu_status = 3 --補修
            //			inner join s04_subject on
            //				s04_noropenc.sub_id = s04_subject.sub_id
            //			inner join s90_organization on
            //				s04_noropenc.org_id = s90_organization.org_id
            //			inner join s04_hstusubjscoreterm on
            //				s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
            //				s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
            //				s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
            //				s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
            //				s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
            //				s04_hstusubjscoreterm.hstusst_status = 3
            //			left join s04_hstusixscoreterm on
            //				s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
            //				s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
            //				s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
            //			left join s04_stubuterm on
            //				s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
            //				s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
            //				s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
            //				s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
            //			inner join s04_stddbgo on
            //				s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
            //				s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
            //				s04_stuhcls.deg_id = s04_stddbgo.deg_id and
            //				s04_stuhcls.dep_id = s04_stddbgo.dep_id and
            //				s04_stuhcls.bra_id = s04_stddbgo.bra_id and
            //				s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
            //				s04_stuhcls.cg_code = s04_stddbgo.cg_code and
            //				isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
            //				s04_noropenc.sub_id = s04_stddbgo.sub_id and
            //				( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
            //			inner join s04_108subject on
            //				s04_stddbgo.course_code = s04_108subject.course_code
            //			inner join s04_ytdbgoc on
            //				s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 = s04_ytdbgoc.year_id and
            //				s04_stddbgo.sms_id = s04_ytdbgoc.sms_id and
            //				s04_stddbgo.dep_id = s04_ytdbgoc.dep_id and
            //				s04_stddbgo.deg_id = s04_ytdbgoc.deg_id and
            //				s04_stddbgo.bra_id = s04_ytdbgoc.bra_id and
            //				s04_stddbgo.grd_id = s04_ytdbgoc.grd_id and
            //				s04_stuhcls.cls_id = s04_ytdbgoc.cls_id
            //			inner join s90_organization org_o on
            //				s04_ytdbgoc.org_id = org_o.org_id
            //			inner join s90_branch on
            //				s04_ytdbgoc.bra_id = s90_branch.bra_id
            //			inner join s04_108subject subj108_o on
            //				s04_stddbgo.course_code = subj108_o.course_code
            //			left join L01_std_attestation a on
            //				a.year_id = s04_stuhcls.year_id and
            //				a.sms_id = s04_stuhcls.sms_id and
            //				a.cls_id = convert(varchar(6),s04_noropenc.noroc_id) and
            //				a.sub_id = convert(varchar(10),s04_subject.sub_id) and
            //				a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
            //				a.emp_id =  convert(varchar(50),s04_noropenc.all_empid)and
            //				a.std_no = s04_student.std_no
            //			where s04_student.std_hisdatayear >= 108
            //			) a
            //                            ) AS NewTable
            //                        WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun
            //                        order by as_cls_uid,as_course_id";
            #endregion

            #region insert into L01_std_attestation
            string str_sql = @"
												with temp(sch_no,year_id,sms_id,cls_id,sub_id,src_dup,emp_id,std_no,ser_id,
													attestation_send,attestation_date,attestation_status,attestation_centraldb,
													content,is_sys,
													credit,grd_id,in_sms_id,borrow_yn,
													reread_yn,reread_yms,reread_type,repair_yn,repair_yms,repair_type,
													reread2_yn,reread2_yms,turn_yn,upyms,create_dt,yms,actually_year,actually_sms,sub_name,all_empname,stu_status,cls_name)
												as
												(
												SELECT 
												sch_no = s04_student.sch_no,
												year_id = s04_stuhcls.year_id,
												sms_id = s04_stuhcls.sms_id,
												cls_id = 
												convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
														convert(varchar(1),s04_noropenc.dep_id) + 
														case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
														convert(varchar(1),s04_noropenc.grd_id) + 
														case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
												sub_id = convert(varchar(10),s04_subject.sub_id),
												src_dup = convert(varchar(23),s04_stddbgo.course_code),
												emp_id = convert(varchar(50),s04_noropenc.all_empid),
												std_no = s04_student.std_no,
												ser_id = 1,
												attestation_send = '',
												attestation_date = '',
												attestation_status = '',
												attestation_centraldb = 'N',
												content = '',
												is_sys = '2',
												credit = convert(varchar(10),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
												grd_id = convert(smallint,s04_stuhcls.grd_id),
												in_sms_id = s04_stuhcls.sms_id,
												borrow_yn = 'N',
												reread_yn = 'N',
												reread_yms = '0000',
												reread_type = '0',
												repair_yn  = 'N',
												repair_yms = '0000',
												repair_type = '0',
												reread2_yn = 'N',
												reread2_yms = '0000',
												turn_yn = 'N',
												upyms = '',
												create_dt = '',
												yms = '0000',
												actually_year = s04_stuhcls.year_id,
												actually_sms = s04_stuhcls.sms_id,
												sub_name = s04_108subject.sub108_name,
												all_empname = s04_noropenc.all_empname,
												stu_status = '1',
												cls_name = s04_noropenc.noroc_clsname
												FROM s04_student
												inner join s04_stuhcls on
													s04_student.std_no = s04_stuhcls.std_no and												
													s04_stuhcls.sch_no = @sch_no and 
													s04_stuhcls.year_id = @year_id and 
													s04_stuhcls.sms_id = @sms_id and
													s04_student.std_no = @std_no
												inner join s90_cha_id on
													s04_stuhcls.std_status = s90_cha_id.cha_id and
													s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
												inner join s04_norstusc on
													s04_student.std_no = s04_norstusc.std_no and
													s04_norstusc.in_year = s04_stuhcls.year_id and
													s04_norstusc.in_sms = s04_stuhcls.sms_id and
													s04_norstusc.stu_status = 1 --新修
												inner join s04_noropenc on
													s04_norstusc.noroc_id = s04_noropenc.noroc_id and
													isnull(s04_noropenc.course_code,'') <> '' and
													isnull(s04_noropenc.is_learn,'N') = 'Y'
												inner join s04_subject on
													s04_noropenc.sub_id = s04_subject.sub_id
												inner join s04_stddbgo on
													s04_stuhcls.year_id - s04_stuhcls.grd_id + 1 = s04_stddbgo.year_id and
													s04_stuhcls.sms_id = s04_stddbgo.sms_id and
													s04_stuhcls.deg_id = s04_stddbgo.deg_id and
													s04_stuhcls.dep_id = s04_stddbgo.dep_id and
													s04_stuhcls.bra_id = s04_stddbgo.bra_id and
													s04_stuhcls.grd_id = s04_stddbgo.grd_id and
													s04_stuhcls.cg_code = s04_stddbgo.cg_code and
													isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
													s04_noropenc.sub_id = s04_stddbgo.sub_id and
													( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
												inner join s04_108subject on
													s04_stddbgo.course_code = s04_108subject.course_code
												left join s90_organization on
													s04_noropenc.org_id = s90_organization.org_id
												left join s90_branch on
													s04_noropenc.bra_id = s90_branch.bra_id
												left join s04_hstusubjscoreterm on
													s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
													s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
													s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
													s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
													s04_hstusubjscoreterm.hstusst_status = 1
												left join s04_hstusixscoreterm on
													s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
													s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
													s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
												left join s04_stubuterm on
													s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
													s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
													s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
													s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
												where s04_student.std_hisdatayear >= 108

												union 

												SELECT
												sch_no = s04_student.sch_no,
												year_id = s04_stuhcls.year_id,
												sms_id = s04_stuhcls.sms_id,
												cls_id = convert(varchar(6),s04_noropenc.noroc_id),
												sub_id = convert(varchar(10),s04_subject.sub_id),
												src_dup = convert(varchar(23),s04_stddbgo.course_code),
												emp_id = convert(varchar(50),s04_noropenc.all_empid),
												std_no = s04_student.std_no,
												ser_id = 1,
												attestation_send = '',
												attestation_date = '',
												attestation_status = '',
												attestation_centraldb = 'N',
												content = '',
												is_sys = '2',
												credit =  convert(varchar , s04_noropenc.sub_credit) ,
												grd_id = convert(varchar(1) , noro_o.grd_id ),
												in_sms_id = s04_stuhcls.sms_id,
												borrow_yn = 'N',
												reread_yn = 'Y',
												reread_yms = convert(varchar,s04_stuhcls.year_id)+convert(varchar,s04_stuhcls.sms_id),
												reread_type = '1',
												repair_yn  = 'N',
												repair_yms = '0000',
												repair_type = '0',
												reread2_yn = 'N',
												reread2_yms = '0000',
												turn_yn = 'N',
												upyms = '',
												create_dt = '',
												yms = '0000',
												actually_year = convert(smallint , s04_hstusubjscoreterm.in_year ),
												actually_sms = convert(smallint , s04_hstusubjscoreterm.in_sms ),
												sub_name = s04_108subject.sub108_name,
												all_empname = s04_noropenc.all_empname,
												stu_status = '2',
												cls_name = s04_noropenc.noroc_clsname
												FROM s04_student
												inner join s04_stuhcls on
													s04_student.std_no = s04_stuhcls.std_no and												
													s04_stuhcls.sch_no = @sch_no and 
													s04_stuhcls.year_id = @year_id and 
													s04_stuhcls.sms_id = @sms_id and
													s04_student.std_no = @std_no
												inner join s90_cha_id on
													s04_stuhcls.std_status = s90_cha_id.cha_id and
													s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370') --只上傳正常學籍、休學、輔導延修
												inner join s04_noropenc on
													s04_noropenc.year_id = s04_stuhcls.year_id and
													s04_noropenc.sms_id = s04_stuhcls.sms_id
												inner join s04_norstusc on
													s04_norstusc.noroc_id = s04_noropenc.noroc_id and
													s04_student.std_no = s04_norstusc.std_no and
													s04_norstusc.stu_status = 2 --重修
												inner join s04_subject on
													s04_noropenc.sub_id = s04_subject.sub_id
												inner join s04_hstusubjscoreterm on
													s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
													s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
													s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
													s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
													s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
													s04_hstusubjscoreterm.hstusst_status = 2
												inner join s04_stddbgo on
													s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
													s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
													s04_stuhcls.deg_id = s04_stddbgo.deg_id and
													s04_stuhcls.dep_id = s04_stddbgo.dep_id and
													s04_stuhcls.bra_id = s04_stddbgo.bra_id and
													s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
													s04_stuhcls.cg_code = s04_stddbgo.cg_code and
													isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
													s04_noropenc.sub_id = s04_stddbgo.sub_id and
													( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
												inner join s04_108subject on
													s04_stddbgo.course_code = s04_108subject.course_code
												inner join s90_organization on
													s04_noropenc.org_id = s90_organization.org_id
												inner join s90_branch on
													s04_noropenc.bra_id = s90_branch.bra_id
												inner join s04_hstusixscoreterm on
													s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
													s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
													s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
												left join s04_stubuterm on
													s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
													s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
													s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
													s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id 
												inner join s04_hstusubjscoreterm hstusst_new on
													s04_hstusubjscoreterm.sub_id = hstusst_new.sub_id and
													s04_hstusubjscoreterm.std_no = hstusst_new.std_no and
													s04_hstusubjscoreterm.in_year = hstusst_new.in_year and
													s04_hstusubjscoreterm.in_sms = hstusst_new.in_sms and
													hstusst_new.hstusst_status = 1
												inner join s04_norstusc norstu_o on
													hstusst_new.std_no = norstu_o.std_no and
													hstusst_new.in_year = norstu_o.in_year and
													hstusst_new.in_sms = norstu_o.in_sms and
													norstu_o.stu_status = 1
												inner join s04_noropenc noro_o on
													norstu_o.noroc_id = noro_o.noroc_id and
													hstusst_new.sub_id = noro_o.sub_id 		
												inner join s90_organization org_o on
													noro_o.org_id = org_o.org_id
												where s04_student.std_hisdatayear >= 108 

												union

												SELECT
												sch_no = s04_student.sch_no,
												year_id = s04_stuhcls.year_id,
												sms_id = s04_stuhcls.sms_id,
												cls_id = convert(varchar(6),s04_noropenc.noroc_id),
												sub_id = convert(varchar(10),s04_subject.sub_id),
												src_dup = convert(varchar(23),s04_stddbgo.course_code),
												emp_id = convert(varchar(50),s04_noropenc.all_empid),
												std_no = s04_student.std_no,
												ser_id = 1,
												attestation_send = '',
												attestation_date = '',
												attestation_status = '',
												attestation_centraldb = 'N',
												content = '',
												is_sys = '2',
												credit = convert(varchar , s04_stddbgo.sub_credit) ,
												grd_id = convert(smallint,s04_stuhcls.grd_id),
												in_sms_id = s04_stuhcls.sms_id,
												borrow_yn = 'N',
												reread_yn = 'N',
												reread_yms = '0000',
												reread_type = '0',
												repair_yn  = 'Y',
												repair_yms = convert(varchar , s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 )+convert(varchar , s04_stddbgo.sms_id ),
												repair_type = '1',
												reread2_yn = 'N',
												reread2_yms = '0000',
												turn_yn = 'N',
												upyms = '',
												create_dt = '',
												yms = '0000',
												actually_year = convert(smallint , s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 ),
												actually_sms =  convert(smallint , s04_stddbgo.sms_id ),
												sub_name = subj108_o.sub108_name,
												all_empname = s04_noropenc.all_empname,
												stu_status = '3',
												cls_name = s04_noropenc.noroc_clsname
												FROM s04_student
												inner join s04_stuhcls on
													s04_student.std_no = s04_stuhcls.std_no and												
													s04_stuhcls.sch_no = @sch_no and 
													s04_stuhcls.year_id = @year_id and 
													s04_stuhcls.sms_id = @sms_id and
													s04_student.std_no = @std_no
												inner join s90_cha_id on
													s04_stuhcls.std_status = s90_cha_id.cha_id and
													s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
												inner join s04_noropenc on
													s04_noropenc.year_id = s04_stuhcls.year_id and
													s04_noropenc.sms_id = s04_stuhcls.sms_id 
												inner join s04_norstusc on
													s04_norstusc.noroc_id = s04_noropenc.noroc_id and
													s04_student.std_no = s04_norstusc.std_no and
													s04_norstusc.stu_status = 3 --補修
												inner join s04_subject on
													s04_noropenc.sub_id = s04_subject.sub_id
												inner join s90_organization on
													s04_noropenc.org_id = s90_organization.org_id
												inner join s04_hstusubjscoreterm on
													s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
													s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
													s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
													s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
													s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
													s04_hstusubjscoreterm.hstusst_status = 3
												left join s04_hstusixscoreterm on
													s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
													s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
													s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
												left join s04_stubuterm on
													s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
													s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
													s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
													s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
												inner join s04_stddbgo on
													s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
													s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
													s04_stuhcls.deg_id = s04_stddbgo.deg_id and
													s04_stuhcls.dep_id = s04_stddbgo.dep_id and
													s04_stuhcls.bra_id = s04_stddbgo.bra_id and
													s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
													s04_stuhcls.cg_code = s04_stddbgo.cg_code and
													isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
													s04_noropenc.sub_id = s04_stddbgo.sub_id and
													( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
												inner join s04_108subject on
													s04_stddbgo.course_code = s04_108subject.course_code
												inner join s04_ytdbgoc on
													s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 = s04_ytdbgoc.year_id and
													s04_stddbgo.sms_id = s04_ytdbgoc.sms_id and
													s04_stddbgo.dep_id = s04_ytdbgoc.dep_id and
													s04_stddbgo.deg_id = s04_ytdbgoc.deg_id and
													s04_stddbgo.bra_id = s04_ytdbgoc.bra_id and
													s04_stddbgo.grd_id = s04_ytdbgoc.grd_id and
													s04_stuhcls.cls_id = s04_ytdbgoc.cls_id
												inner join s90_organization org_o on
													s04_ytdbgoc.org_id = org_o.org_id
												inner join s90_branch on
													s04_ytdbgoc.bra_id = s90_branch.bra_id
												inner join s04_108subject subj108_o on
													s04_stddbgo.course_code = subj108_o.course_code
												where s04_student.std_hisdatayear >= 108
												)
												insert into L01_std_attestation
												(	sch_no,year_id,sms_id,cls_id,sub_id,src_dup,emp_id,std_no,ser_id,
													attestation_send,attestation_date,attestation_status,attestation_centraldb,
													content,is_sys,
													credit,grd_id,in_sms_id,borrow_yn,
													reread_yn,reread_yms,reread_type,repair_yn,repair_yms,repair_type,
													reread2_yn,reread2_yms,turn_yn,upyms,create_dt,yms,actually_year,actually_sms,sub_name,all_empname,stu_status,cls_name)
												select * 
												from temp
												where not exists(
																	select 1 
																	from L01_std_attestation a
																	where a.sch_no = temp.sch_no
																	and a.year_id = temp.year_id
																	and a.sms_id = temp.sms_id
																	and a.cls_id = temp.cls_id
																	and a.sub_id = temp.sub_id
																	and a.src_dup = temp.src_dup
																	and a.emp_id = temp.emp_id
																	and a.std_no = temp.std_no
																	and a.ser_id = temp.ser_id
																	)";
			try 
			{
                using (IDbConnection conn = _context.CreateCommand())
                {
                    await conn.ExecuteAsync(str_sql, arg);
                }
            }
			catch (Exception ex) 
			{
			}
            #endregion

            str_sql = @"
											SELECT 
											*,
											x_cnt = 
												(select count(*)
												 from L01_std_public_filehub a
												 where a.class_name = 'StuAttestation'
												 and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0'),
											x_total =
											(select count(*)
											from L01_std_attestation a
											where a.year_id = NewTable.b
											and   a.sms_id = NewTable.c
											and   a.std_no = NewTable.h),
											x_file_cnt =
														(select value
														 from L01_setup
														 where id = 2)
											FROM(
											select *,
												   ROW_NUMBER() OVER(ORDER BY a.b,a.c,a.d,a.e,a.f) AS RowNum
											from (
														select 
														a = sch_no,
														b = year_id,
														c = sms_id ,
														d = cls_id,
														e = sub_id,
														f = src_dup,
														g = emp_id,
														h = std_no,
														i = ser_id,
														j = credit,
														k = '',
														l = case 
															when isnull(attestation_send,'') = '' then '' 
															else	substring(attestation_send,1,3)+'/'+
																	substring(attestation_send,4,2)+'/'+
																	substring(attestation_send,6,2)+' '+
																	substring(attestation_send,8,2)+':'+
																	substring(attestation_send,10,2)+':'+
																	substring(attestation_send,12,2)
															end,
														m = case
															when isnull(attestation_release,'') = '' then '' 
															else	substring(attestation_date,1,3)+'/'+
																	substring(attestation_date,4,2)+'/'+
																	substring(attestation_date,6,2)+' '+
																	substring(attestation_date,8,2)+':'+
																	substring(attestation_date,10,2)+':'+
																	substring(attestation_date,12,2)
															end,
														n = case when isnull(attestation_release,'') = '' and isnull(attestation_status,'') = '' then '' else attestation_status end,
														o = is_sys,
														p = content,
														q = sub_name,
														r = all_empname,
														s = 
														(
														Select SUBSTRING(
															(Select ','+ file_name
															 From L01_std_public_filehub b
															 where 
																b.complex_key = L01_std_attestation.sch_no+'_'+
																				convert(varchar,L01_std_attestation.year_id)+'_'+
																				convert(varchar,L01_std_attestation.sms_id)+'_'+
																				L01_std_attestation.cls_id+'_'+
																				L01_std_attestation.sub_id+'_'+
																				L01_std_attestation.src_dup+'_'+
																				L01_std_attestation.emp_id+'_'+
																				L01_std_attestation.std_no+'_0' and
																b.class_name = 'StuAttestation' 
															For Xml Path(''))
														, 2, 8000)
														),
														w = case isnull(attestation_reason,'')
															when '01' then '非本課程相關主題成果，無法認證'
															when '02' then '無法查證成果可信度'
															when '03' then '請加強成果內容正確性'
															when '04' then '請增加成果內容豐富度'  else '' end,
														x = attestation_release
														from L01_std_attestation
														where 										
														sch_no = @sch_no and 
														year_id = @year_id and 
														sms_id = @sms_id and
														std_no = @std_no
														)a
											)as NewTable
											where RowNum >= @sRowNun AND RowNum <= @eRowNun
											order by
											d,e";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<HeaderList>(str_sql, arg);
            }
        }
        public async Task<IEnumerable<HeaderList>> GetListConfirm(StuAttestationQueryList arg)
        {
            arg.sch_no = _context.SchNo;
			#region old sql
			//            string str_sql = @"
			//			                       SELECT *,
			//			                           (select count(*)
			//			                           from L01_std_public_filehub a
			//			                           where a.class_name = 'StuAttestation'
			//			                           and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0') as x_cnt,
			//										x_total =
			//													(select count(*)
			//														from L01_std_attestation a
			//														where a.sch_no = NewTable.a
			//														and a.year_id = NewTable.b
			//														and a.sms_id = NewTable.c
			//														and a.std_no = NewTable.h
			//														and isnull(a.attestation_send,'') <>'' 
			//														and isnull(a.attestation_status,'') = 'Y'
			//														),
			//										'' as x_status,
			//                                        x_file_center_cnt = 
			///*
			//                                        (select convert(integer,value)
			//                                        from L01_setup
			//                                        where id = 1)
			//										-*/
			//										isnull(
			//													(select  count(*)
			//													from L01_std_attestation_file a
			//													where a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0'
			//													and a.class_name = 'StuAttestation'
			//													and a.check_yn = 'Y'),0
			//										)
			//			                       FROM(
			//				select *,
			//						ROW_NUMBER() OVER(ORDER BY a.b,a.c,a.as_cls_uid,a.as_subj_uid) AS RowNum
			//				from (
			//				SELECT 
			//						as_id_no = convert(varchar(10),s04_student.std_identity) ,		--身份證字號
			//						as_std_no = convert(varchar(10),s04_student.std_no),			--學號
			//						as_grade = convert(smallint,s04_stuhcls.grd_id) ,				--學生年級
			//						as_cls_uid = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
			//							convert(varchar(1),s04_noropenc.dep_id) + 
			//							case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
			//							convert(varchar(1),s04_noropenc.grd_id) + 
			//							case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
			//						 as_cls_no = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級代碼
			//							convert(varchar(1),s04_noropenc.dep_id) + 
			//							case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
			//							convert(varchar(1),s04_noropenc.grd_id) + 
			//							case when s04_noropenc.cls_id >= 10 then '0' + convert(varchar(2),s04_noropenc.cls_id) else '00' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
			//						as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname), --開課班級名稱
			//						as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end), --開課班級內分組
			//						as_subj_uid = convert(varchar(10),s04_subject.sub_id), --科目內碼
			//						as_course_id = convert(varchar(23),s04_stddbgo.course_code), --課程計畫平臺核發之課程代碼	
			//						as_subj_name = convert(varchar(90),s04_108subject.sub108_name), --科目名稱
			//						as_credits = convert(varchar(10),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end), --學分/時數
			//						as_type = convert(varchar(1),s04_norstusc.stu_status),	--修課類別
			//						as_use_credits = convert(varchar(1),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ), --是否採計學分
			//						as_o_syear =null, --原(應)修課學年度
			//						as_o_sems = null, --原(應)修課學期
			//						as_o_cls_year = null, --原(應)開課年級
			//						as_o_cls_no = null, --原(應)開課班級代碼
			//						as_o_cls_name = null, --原(應)開課班級名稱
			//						as_o_cls_group = null, --原(應)開課班級內分組
			//						as_o_subj_id = null, --原(應)修校務系統科目內碼
			//						as_o_course_id = null , --原(應)修課程代碼
			//						as_o_subj_name = null, --原(應)修科目名稱
			//						as_o_credits = null, --原(應)修學分
			//						as_makeup_mode = null, --補修方式
			//						as_rebuild_mode = null, --重修方式
			//						as_is_precourse = '0', --是否為預選課程
			//						as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid), --授課教師內碼
			//						as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode), --授課教師代碼
			//									as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname), --授課教師姓名
			//						s04_stuhcls.sch_no as a,
			//						s04_stuhcls.year_id as b,
			//						s04_stuhcls.sms_id as c,
			//						d = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
			//							convert(varchar(1),s04_noropenc.dep_id) + 
			//							case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
			//							convert(varchar(1),s04_noropenc.grd_id) + 
			//							case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
			//						e = convert(varchar(10),s04_subject.sub_id),
			//						f = convert(varchar(23),s04_stddbgo.course_code),
			//						g = convert(varchar(50),s04_noropenc.all_empid),
			//						h = s04_student.std_no ,
			//						a.ser_id as i,
			//						s04_noropenc.sub_credit as j,
			//						convert(varchar(1),s04_norstusc.stu_status) as k,
			//						case when isnull(a.attestation_send,'') = '' then '' 
			//						else substring(a.attestation_send,1,3)+'/'+
			//						substring(a.attestation_send,4,2)+'/'+
			//						substring(a.attestation_send,6,2)+' '+
			//						substring(a.attestation_send,8,2)+':'+
			//						substring(a.attestation_send,10,2)+':'+
			//						substring(a.attestation_send,12,2)
			//						end  as l,
			//						case when isnull(a.attestation_release,'') = '' then '' 
			//						else substring(a.attestation_date,1,3)+'/'+
			//						substring(a.attestation_date,4,2)+'/'+
			//						substring(a.attestation_date,6,2)+' '+
			//						substring(a.attestation_date,8,2)+':'+
			//						substring(a.attestation_date,10,2)+':'+
			//						substring(a.attestation_date,12,2)
			//						end  as m,
			//						a.attestation_status as n,
			//						a.is_sys as o,
			//						a.content as p,
			//						convert(varchar(90),s04_108subject.sub108_name) as q,
			//						convert(varchar(50),s04_noropenc.all_empname) as r,
			//						s = 
			//						(
			//						Select SUBSTRING(
			//							(Select ','+ file_name
			//							From L01_std_public_filehub b
			//						where 
			//								b.complex_key = s04_stuhcls.sch_no+'_'+
			//												convert(varchar,s04_stuhcls.year_id)+'_'+
			//												convert(varchar,s04_stuhcls.sms_id)+'_'+
			//												convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
			//												convert(varchar(1),s04_noropenc.dep_id) + 
			//												case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
			//												convert(varchar(1),s04_noropenc.grd_id) + 
			//												case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end )+'_'+
			//													convert(varchar(10),s04_subject.sub_id)+'_'+
			//												convert(varchar(23),s04_stddbgo.course_code)+'_'+
			//												convert(varchar(50),s04_noropenc.all_empid)+'_'+
			//												s04_student.std_no+'_0' and
			//								b.class_name = 'StuAttestation' 
			//							For Xml Path(''))
			//						, 2, 8000)
			//						),
			//						u = a.attestation_confirm,
			//						v = a.attestation_centraldb
			//					FROM s04_student
			//					inner join s04_stuhcls on
			//						s04_student.std_no = s04_stuhcls.std_no and												
			//						s04_stuhcls.sch_no = @sch_no and 
			//						s04_stuhcls.year_id = @year_id and 
			//						s04_stuhcls.sms_id = @sms_id and
			//						s04_student.std_no = @std_no
			//					inner join s90_cha_id on
			//						s04_stuhcls.std_status = s90_cha_id.cha_id and
			//						s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
			//					inner join s04_norstusc on
			//						s04_student.std_no = s04_norstusc.std_no and
			//						s04_norstusc.in_year = s04_stuhcls.year_id and
			//						s04_norstusc.in_sms = s04_stuhcls.sms_id and
			//						s04_norstusc.stu_status = 1 --新修
			//					inner join s04_noropenc on
			//						s04_norstusc.noroc_id = s04_noropenc.noroc_id and
			//						isnull(s04_noropenc.course_code,'') <> '' and
			//						isnull(s04_noropenc.is_learn,'N') = 'Y'
			//					inner join s04_subject on
			//						s04_noropenc.sub_id = s04_subject.sub_id
			//					inner join s04_stddbgo on
			//						s04_stuhcls.year_id - s04_stuhcls.grd_id + 1 = s04_stddbgo.year_id and
			//						s04_stuhcls.sms_id = s04_stddbgo.sms_id and
			//						s04_stuhcls.deg_id = s04_stddbgo.deg_id and
			//						s04_stuhcls.dep_id = s04_stddbgo.dep_id and
			//						s04_stuhcls.bra_id = s04_stddbgo.bra_id and
			//						s04_stuhcls.grd_id = s04_stddbgo.grd_id and
			//						s04_stuhcls.cg_code = s04_stddbgo.cg_code and
			//						isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
			//						s04_noropenc.sub_id = s04_stddbgo.sub_id and
			//						( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
			//					inner join s04_108subject on
			//						s04_stddbgo.course_code = s04_108subject.course_code
			//					inner join L01_std_attestation a on
			//						a.year_id = s04_stuhcls.year_id and
			//						a.sms_id = s04_stuhcls.sms_id and
			//						a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
			//								convert(varchar(1),s04_noropenc.dep_id) + 
			//								case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
			//								convert(varchar(1),s04_noropenc.grd_id) + 
			//								case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ) and
			//						a.sub_id = convert(varchar(10),s04_subject.sub_id) and
			//						a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
			//						a.emp_id =  convert(varchar(50),s04_noropenc.all_empid) and
			//						isnull(a.attestation_send,'') <>'' and 
			//						isnull(a.attestation_status,'') = 'Y'and
			//						isnull(a.attestation_release,'') = 'Y'and
			//						a.std_no = s04_student.std_no
			//					left join s90_organization on
			//						s04_noropenc.org_id = s90_organization.org_id
			//					left join s90_branch on
			//						s04_noropenc.bra_id = s90_branch.bra_id
			//					left join s04_hstusubjscoreterm on
			//						s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
			//						s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
			//						s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
			//						s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
			//						s04_hstusubjscoreterm.hstusst_status = 1
			//					left join s04_hstusixscoreterm on
			//						s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
			//						s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
			//						s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
			//					left join s04_stubuterm on
			//						s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
			//						s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
			//						s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
			//						s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
			//					where s04_student.std_hisdatayear >= 108
			//				union
			//				SELECT 
			//						as_id_no = convert(varchar(10),s04_student.std_identity) ,
			//						as_std_no = convert(varchar(10),s04_student.std_no),
			//						as_grade = convert(smallint,s04_stuhcls.grd_id) ,
			//						as_cls_uid = convert(varchar(6),s04_noropenc.noroc_id),
			//						as_cls_no = convert(varchar(6),s04_noropenc.noroc_id),
			//						as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname),
			//						as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end),
			//						as_subj_uid = convert(varchar(10),s04_subject.sub_id),
			//						as_course_id = convert(varchar(23),s04_stddbgo.course_code),
			//						as_subj_name = convert(varchar(90),s04_108subject.sub108_name),
			//						as_credits = convert(varchar(1),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
			//						as_type = convert(varchar(1),s04_norstusc.stu_status),
			//						as_use_credits = convert(varchar(10),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ),
			//						as_o_syear = convert(smallint , s04_hstusubjscoreterm.in_year ),
			//						as_o_seme = convert(smallint , s04_hstusubjscoreterm.in_sms ),
			//						as_o_cls_year = convert(varchar(1) , noro_o.grd_id ),
			//						as_o_cls_no = convert(varchar(1),noro_o.dep_id) + 
			//							case when noro_o.bra_id >= 10 then convert(varchar(2),noro_o.bra_id) else '0' + convert(varchar(1),noro_o.bra_id) end + 
			//							convert(varchar(1),noro_o.grd_id) + 
			//							case when noro_o.cls_id >= 100 then convert(varchar(3),noro_o.cls_id) when noro_o.cls_id >= 10 then '0' + convert(varchar(2),noro_o.cls_id) else '00' + convert(varchar(1),noro_o.cls_id) end,
			//						 as_o_cls_name = convert(varchar(90),noro_o.noroc_clsname),
			//						as_o_cls_group = convert(varchar(20),case when isnull(noro_o.org_id,0) in (0,99) then '無' else org_o.org_name end) ,
			//						as_o_subj_id = convert(varchar(10),noro_o.sub_id),
			//						as_o_course_id = convert(varchar(23),noro_o.course_code),
			//						as_o_subj_name = convert(varchar(90),s04_108subject.sub108_name),
			//						as_o_credits = convert(smallint , s04_noropenc.sub_credit) ,
			//						as_makeup_mode = null,
			//						as_rebuild_mode = convert(varchar(1),case isnull(s04_hstusubjscoreterm.hstusst_outsch,'0') when '1' then 4 when '2' then 5 when '3' then 6 end),
			//						as_is_precourse = '0',
			//						as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid),
			//						as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode),
			//									as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
			//						s04_stuhcls.sch_no as a,
			//						s04_stuhcls.year_id as b,
			//						s04_stuhcls.sms_id as c,
			//						d = convert(varchar(6),s04_noropenc.noroc_id),
			//						e = convert(varchar(10),s04_subject.sub_id),
			//						f = convert(varchar(23),s04_stddbgo.course_code),
			//						g = convert(varchar(50),s04_noropenc.all_empid),
			//						h = s04_student.std_no ,
			//						a.ser_id as i,
			//						s04_noropenc.sub_credit as j,
			//						convert(varchar(1),s04_norstusc.stu_status) as k,
			//						case when isnull(a.attestation_send,'') = '' then '' 
			//						else substring(a.attestation_send,1,3)+'/'+
			//						substring(a.attestation_send,4,2)+'/'+
			//						substring(a.attestation_send,6,2)+' '+
			//						substring(a.attestation_send,8,2)+':'+
			//						substring(a.attestation_send,10,2)+':'+
			//						substring(a.attestation_send,12,2)
			//						end  as l,
			//						case when isnull(a.attestation_release,'') = '' then '' 
			//						else substring(a.attestation_date,1,3)+'/'+
			//						substring(a.attestation_date,4,2)+'/'+
			//						substring(a.attestation_date,6,2)+' '+
			//						substring(a.attestation_date,8,2)+':'+
			//						substring(a.attestation_date,10,2)+':'+
			//						substring(a.attestation_date,12,2)
			//						end  as m,
			//						a.attestation_status as n,
			//						a.is_sys as o,
			//						a.content as p,
			//						convert(varchar(90),s04_108subject.sub108_name) as q,
			//						convert(varchar(50),s04_noropenc.all_empname) as r,
			//						s = 
			//						(
			//						Select SUBSTRING(
			//							(Select ','+ file_name
			//							From L01_std_public_filehub b
			//						where 
			//								b.complex_key = s04_stuhcls.sch_no+'_'+
			//												convert(varchar,s04_stuhcls.year_id)+'_'+
			//												convert(varchar,s04_stuhcls.sms_id)+'_'+
			//												convert(varchar(6),s04_noropenc.noroc_id)+'_'+
			//												convert(varchar(10),s04_subject.sub_id)+'_'+
			//												convert(varchar(23),s04_stddbgo.course_code)+'_'+
			//												convert(varchar(50),s04_noropenc.all_empid)+'_'+
			//												s04_student.std_no+'_0' and
			//								b.class_name = 'StuAttestation' 
			//							For Xml Path(''))
			//						, 2, 8000)
			//						),
			//						u = a.attestation_confirm,
			//						v = a.attestation_centraldb
			//					FROM s04_student
			//					inner join s04_stuhcls on
			//						s04_student.std_no = s04_stuhcls.std_no and												
			//						s04_stuhcls.sch_no = @sch_no and 
			//						s04_stuhcls.year_id = @year_id and 
			//						s04_stuhcls.sms_id = @sms_id and
			//						s04_student.std_no = @std_no
			//					inner join s90_cha_id on
			//						s04_stuhcls.std_status = s90_cha_id.cha_id and
			//						s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370') --只上傳正常學籍、休學、輔導延修
			//					inner join s04_noropenc on
			//						s04_noropenc.year_id = s04_stuhcls.year_id and
			//						s04_noropenc.sms_id = s04_stuhcls.sms_id
			//					inner join s04_norstusc on
			//						s04_norstusc.noroc_id = s04_noropenc.noroc_id and
			//						s04_student.std_no = s04_norstusc.std_no and
			//						s04_norstusc.stu_status = 2 --重修
			//					inner join s04_subject on
			//						s04_noropenc.sub_id = s04_subject.sub_id
			//					inner join s04_hstusubjscoreterm on
			//						s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
			//						s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
			//						s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
			//						s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
			//						s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
			//						s04_hstusubjscoreterm.hstusst_status = 2
			//					inner join s04_stddbgo on
			//						s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
			//						s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
			//						s04_stuhcls.deg_id = s04_stddbgo.deg_id and
			//						s04_stuhcls.dep_id = s04_stddbgo.dep_id and
			//						s04_stuhcls.bra_id = s04_stddbgo.bra_id and
			//						s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
			//						s04_stuhcls.cg_code = s04_stddbgo.cg_code and
			//						isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
			//						s04_noropenc.sub_id = s04_stddbgo.sub_id and
			//						( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
			//					inner join s04_108subject on
			//						s04_stddbgo.course_code = s04_108subject.course_code
			//					inner join s90_organization on
			//						s04_noropenc.org_id = s90_organization.org_id
			//					inner join s90_branch on
			//						s04_noropenc.bra_id = s90_branch.bra_id
			//					inner join s04_hstusixscoreterm on
			//						s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
			//						s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
			//						s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
			//					 join L01_std_attestation a on
			//						a.year_id = s04_stuhcls.year_id and
			//						a.sms_id = s04_stuhcls.sms_id and
			//						a.cls_id = convert(varchar(6),s04_noropenc.noroc_id) and
			//						a.sub_id = convert(varchar(10),s04_subject.sub_id) and
			//						a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
			//						a.emp_id =  convert(varchar(50),s04_noropenc.all_empid) and
			//						isnull(a.attestation_send,'') <>''   and
			//						isnull(a.attestation_status,'') = 'Y' and
			//						isnull(a.attestation_release,'') = 'Y'and
			//						a.std_no = s04_student.std_no
			//					left join s04_stubuterm on
			//						s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
			//						s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
			//						s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
			//						s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id 
			//					inner join s04_hstusubjscoreterm hstusst_new on
			//						s04_hstusubjscoreterm.sub_id = hstusst_new.sub_id and
			//						s04_hstusubjscoreterm.std_no = hstusst_new.std_no and
			//						s04_hstusubjscoreterm.in_year = hstusst_new.in_year and
			//						s04_hstusubjscoreterm.in_sms = hstusst_new.in_sms and
			//						hstusst_new.hstusst_status = 1
			//					inner join s04_norstusc norstu_o on
			//						hstusst_new.std_no = norstu_o.std_no and
			//						hstusst_new.in_year = norstu_o.in_year and
			//						hstusst_new.in_sms = norstu_o.in_sms and
			//						norstu_o.stu_status = 1
			//					inner join s04_noropenc noro_o on
			//						norstu_o.noroc_id = noro_o.noroc_id and
			//						hstusst_new.sub_id = noro_o.sub_id 		
			//					inner join s90_organization org_o on
			//						noro_o.org_id = org_o.org_id
			//					where s04_student.std_hisdatayear >= 108 
			//				union
			//				SELECT 
			//						as_id_no = convert(varchar(10),s04_student.std_identity) ,
			//						as_std_no = convert(varchar(10),s04_student.std_no),
			//						as_grade = convert(smallint,s04_stuhcls.grd_id) ,
			//						as_cls_uid = convert(varchar(6),s04_noropenc.noroc_id),
			//						as_cls_no = convert(varchar(6),s04_noropenc.noroc_id),
			//						as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname),
			//						as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end),
			//						as_subj_uid = convert(varchar(10),s04_subject.sub_id),
			//						as_course_id = convert(varchar(23),s04_stddbgo.course_code),
			//						as_subj_name = convert(varchar(90),s04_108subject.sub108_name),
			//						as_credits = convert(varchar(1),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
			//						as_type = convert(varchar(1),s04_norstusc.stu_status),
			//						as_use_credits = convert(varchar(10),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ),
			//						as_o_syear = convert(smallint , s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 ),
			//						as_o_sems = convert(smallint , s04_stddbgo.sms_id ),
			//						as_o_cls_year = convert(varchar(1) , s04_stddbgo.grd_id ),
			//						as_o_cls_no = convert(varchar(1),s04_stddbgo.dep_id) + 
			//							case when s04_stddbgo.bra_id >= 10 then convert(varchar(2),s04_stddbgo.bra_id) else '0' + convert(varchar(1),s04_stddbgo.bra_id) end + 
			//							convert(varchar(1),s04_stddbgo.grd_id) + 
			//							case when s04_stuhcls.cls_id >= 10 then '0' + convert(varchar(2),s04_stuhcls.cls_id) else '00' + convert(varchar(1),s04_stuhcls.cls_id) end,
			//						as_o_cls_name = convert(varchar(60),s04_ytdbgoc.cls_abr),
			//						as_o_cls_group = convert(varchar(20),case when isnull(s04_ytdbgoc.org_id,0) in (0,99) then '無' else org_o.org_name end) ,
			//						as_o_subj_id = convert(varchar(10),s04_stddbgo.sub_id),
			//						as_o_course_id = convert(varchar(23),s04_stddbgo.course_code),
			//						as_o_subj_name = convert(varchar(90),subj108_o.sub108_name),
			//						as_o_credits = convert(smallint , s04_stddbgo.sub_credit) ,
			//						as_makeup_mode = convert(varchar(1),case isnull(s04_hstusubjscoreterm.hstusst_outsch,'0') when '1' then 4 when '2' then 5 when '3' then 6 end),
			//						as_rebuild_mode = null,
			//						as_is_precourse = '0',
			//						as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid),
			//						as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode),
			//									as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
			//						s04_stuhcls.sch_no as a,
			//						s04_stuhcls.year_id as b,
			//						s04_stuhcls.sms_id as c,
			//						d = convert(varchar(6),s04_noropenc.noroc_id),
			//						e = convert(varchar(10),s04_subject.sub_id),
			//						f = convert(varchar(23),s04_stddbgo.course_code),
			//						g = convert(varchar(50),s04_noropenc.all_empid),
			//						h = s04_student.std_no ,
			//						a.ser_id as i,
			//						s04_noropenc.sub_credit as j,
			//						convert(varchar(1),s04_norstusc.stu_status) as k,
			//						case when isnull(a.attestation_send,'') = '' then '' 
			//						else substring(a.attestation_send,1,3)+'/'+
			//						substring(a.attestation_send,4,2)+'/'+
			//						substring(a.attestation_send,6,2)+' '+
			//						substring(a.attestation_send,8,2)+':'+
			//						substring(a.attestation_send,10,2)+':'+
			//						substring(a.attestation_send,12,2)
			//						end  as l,
			//						case when isnull(a.attestation_release,'') = '' then '' 
			//						else substring(a.attestation_date,1,3)+'/'+
			//						substring(a.attestation_date,4,2)+'/'+
			//						substring(a.attestation_date,6,2)+' '+
			//						substring(a.attestation_date,8,2)+':'+
			//						substring(a.attestation_date,10,2)+':'+
			//						substring(a.attestation_date,12,2)
			//						end  as m,
			//						a.attestation_status as n,
			//						a.is_sys as o,
			//						a.content as p,
			//						convert(varchar(90),s04_108subject.sub108_name) as q,
			//						convert(varchar(50),s04_noropenc.all_empname) as r,
			//						s = 
			//						(
			//						Select SUBSTRING(
			//							(Select ','+ file_name
			//							From L01_std_public_filehub b
			//						where 
			//								b.complex_key = s04_stuhcls.sch_no+'_'+
			//												convert(varchar,s04_stuhcls.year_id)+'_'+
			//												convert(varchar,s04_stuhcls.sms_id)+'_'+
			//												convert(varchar(6),s04_noropenc.noroc_id)+'_'+
			//												convert(varchar(10),s04_subject.sub_id)+'_'+
			//												convert(varchar(23),s04_stddbgo.course_code)+'_'+
			//												convert(varchar(50),s04_noropenc.all_empid)+'_'+
			//												s04_student.std_no+'_0' and
			//								b.class_name = 'StuAttestation' 
			//							For Xml Path(''))
			//						, 2, 8000)
			//						),
			//						u = a.attestation_confirm,
			//						v = a.attestation_centraldb
			//					FROM s04_student
			//					inner join s04_stuhcls on
			//						s04_student.std_no = s04_stuhcls.std_no and												
			//						s04_stuhcls.sch_no = @sch_no and 
			//						s04_stuhcls.year_id = @year_id and 
			//						s04_stuhcls.sms_id = @sms_id and
			//						s04_student.std_no = @std_no
			//					inner join s90_cha_id on
			//						s04_stuhcls.std_status = s90_cha_id.cha_id and
			//						s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
			//					inner join s04_noropenc on
			//						s04_noropenc.year_id = s04_stuhcls.year_id and
			//						s04_noropenc.sms_id = s04_stuhcls.sms_id 
			//					inner join s04_norstusc on
			//						s04_norstusc.noroc_id = s04_noropenc.noroc_id and
			//						s04_student.std_no = s04_norstusc.std_no and
			//						s04_norstusc.stu_status = 3 --補修
			//					inner join s04_subject on
			//						s04_noropenc.sub_id = s04_subject.sub_id
			//					inner join s90_organization on
			//						s04_noropenc.org_id = s90_organization.org_id
			//					inner join s04_hstusubjscoreterm on
			//						s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
			//						s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
			//						s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
			//						s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
			//						s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
			//						s04_hstusubjscoreterm.hstusst_status = 3
			//					left join s04_hstusixscoreterm on
			//						s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
			//						s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
			//						s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
			//					left join s04_stubuterm on
			//						s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
			//						s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
			//						s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
			//						s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
			//					inner join s04_stddbgo on
			//						s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
			//						s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
			//						s04_stuhcls.deg_id = s04_stddbgo.deg_id and
			//						s04_stuhcls.dep_id = s04_stddbgo.dep_id and
			//						s04_stuhcls.bra_id = s04_stddbgo.bra_id and
			//						s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
			//						s04_stuhcls.cg_code = s04_stddbgo.cg_code and
			//						isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
			//						s04_noropenc.sub_id = s04_stddbgo.sub_id and
			//						( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
			//					inner join s04_108subject on
			//						s04_stddbgo.course_code = s04_108subject.course_code
			//					inner join s04_ytdbgoc on
			//						s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 = s04_ytdbgoc.year_id and
			//						s04_stddbgo.sms_id = s04_ytdbgoc.sms_id and
			//						s04_stddbgo.dep_id = s04_ytdbgoc.dep_id and
			//						s04_stddbgo.deg_id = s04_ytdbgoc.deg_id and
			//						s04_stddbgo.bra_id = s04_ytdbgoc.bra_id and
			//						s04_stddbgo.grd_id = s04_ytdbgoc.grd_id and
			//						s04_stuhcls.cls_id = s04_ytdbgoc.cls_id
			//					inner join s90_organization org_o on
			//						s04_ytdbgoc.org_id = org_o.org_id
			//					inner join s90_branch on
			//						s04_ytdbgoc.bra_id = s90_branch.bra_id
			//					inner join s04_108subject subj108_o on
			//						s04_stddbgo.course_code = subj108_o.course_code
			//					 join L01_std_attestation a on
			//						a.year_id = s04_stuhcls.year_id and
			//						a.sms_id = s04_stuhcls.sms_id and
			//						a.cls_id = convert(varchar(6),s04_noropenc.noroc_id) and
			//						a.sub_id = convert(varchar(10),s04_subject.sub_id) and
			//						a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
			//						a.emp_id =  convert(varchar(50),s04_noropenc.all_empid) and 
			//						isnull(a.attestation_send,'') <>''  and 
			//						isnull(a.attestation_status,'') = 'Y' and
			//						isnull(a.attestation_release,'') = 'Y'and
			//						a.std_no = s04_student.std_no
			//					where s04_student.std_hisdatayear >= 108
			//					) a
			//			                           ) AS NewTable
			//			                       WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun
			//			                       order by as_cls_uid,as_course_id";
			#endregion

			string str_sql = @"
											SELECT 
											*,
											x_cnt = 
												(select count(*)
													from L01_std_public_filehub a
													join L01_std_attestation b on a.complex_key = b.sch_no +'_'+convert(varchar,b.year_id)+'_'+convert(varchar,b.sms_id)+'_'+b.cls_id+'_'+b.sub_id+'_'+b.src_dup+'_'+b.emp_id+'_'+b.std_no+'_0'
													and b.ser_id = a.number_id
													and isnull(b.attestation_send,'') <>''  
													and isnull(b.attestation_status,'') = 'Y'
													and isnull(b.attestation_release,'') = 'Y'
													where a.class_name = 'StuAttestation'
													and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0'),
											x_total =
											(select count(*)
											from L01_std_attestation a
											where a.year_id = NewTable.b
											and   a.sms_id = NewTable.c
											and   a.std_no = NewTable.h
											and isnull(a.attestation_send,'') <>'' 
											and isnull(a.attestation_status,'') = 'Y'),
											x_status = '',
											x_file_center_cnt =
											isnull(
											(select  count(*)
												from L01_std_attestation_file a
												where a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0'
												and a.class_name = 'StuAttestation'
												and a.check_yn = 'Y'),0)
											FROM(
											select *,
												   ROW_NUMBER() OVER(ORDER BY a.b,a.c,a.d,a.e) AS RowNum
											from (
														select  distinct
															a = sch_no,
															b = year_id,
															c = sms_id ,
															d = cls_id,
															e = sub_id,
															f = src_dup,
															g = emp_id,
															h = std_no,
															i = 0,
															j = credit,
															k = stu_status,
															l = '',
															m = '',
															n = '',
															o = '',
															p = '',
															q = sub_name,
															r = all_empname,
															s = 
															(
															Select SUBSTRING(
																(Select ','+ file_name
																	From L01_std_public_filehub b
																	join L01_std_attestation c on b.complex_key = c.sch_no +'_'+convert(varchar,c.year_id)+'_'+convert(varchar,c.sms_id)+'_'+c.cls_id+'_'+c.sub_id+'_'+c.src_dup+'_'+c.emp_id+'_'+c.std_no+'_0'
																		and c.ser_id = b.number_id
																		and isnull(c.attestation_send,'') <>''  
																		and isnull(c.attestation_status,'') = 'Y'
																		and isnull(c.attestation_release,'') = 'Y'　
																	where 
																	b.complex_key = L01_std_attestation.sch_no+'_'+
																					convert(varchar,L01_std_attestation.year_id)+'_'+
																					convert(varchar,L01_std_attestation.sms_id)+'_'+
																					L01_std_attestation.cls_id+'_'+
																					L01_std_attestation.sub_id+'_'+
																					L01_std_attestation.src_dup+'_'+
																					L01_std_attestation.emp_id+'_'+
																					L01_std_attestation.std_no+'_0' and
																	b.class_name = 'StuAttestation'
																For Xml Path(''))
															, 2, 8000)
															),
															w = case isnull(attestation_reason,'')
																when '01' then '非本課程相關主題成果，無法認證'
																when '02' then '無法查證成果可信度'
																when '03' then '請加強成果內容正確性'
																when '04' then '請增加成果內容豐富度'  else '' end,
															x = '',
															u = '',
															v = ''
														from L01_std_attestation
														where 										
														sch_no = @sch_no and 
														year_id = @year_id and 
														sms_id = @sms_id and
														std_no = @std_no and	
														isnull(attestation_send,'') <>'' and 
														isnull(attestation_status,'') = 'Y'and
														isnull(attestation_release,'') = 'Y'
														)a
											)as NewTable
											where RowNum >= @sRowNun AND RowNum <= @eRowNun
											order by
											d,e";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<HeaderList>(str_sql, arg);
            }
        }
        public async Task<IEnumerable<HeaderList>> GetListCentraldb(StuAttestationQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            #region old sql
            //   string str_sql = @"
            //                       SELECT *,
            //                           (select count(*)
            //                           from L01_std_public_filehub a
            //join L01_std_attestation_file b on 
            //																	a.complex_key = b.complex_key and
            //																	a.number_id = b.number_id and
            //																	a.type_id = b.type_id and
            //																	a.class_name = b.class_name and
            //																	b.check_yn = 'Y'
            //                           where a.class_name = 'StuAttestation'
            //                           and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0') as x_cnt,
            //x_total =
            //(select count(*)
            // from L01_std_attestation a
            //	where a.sch_no = NewTable.a
            //	and a.year_id = NewTable.b
            //	and a.sms_id = NewTable.c
            //	and a.std_no = NewTable.h
            //	and isnull(a.attestation_send,'') <>'' 
            //	--and isnull(a.attestation_confirm,'') = 'Y' 
            //	and  isnull(a.attestation_status,'') = 'Y'
            //  )
            //                       FROM(
            //	select *,
            //			ROW_NUMBER() OVER(ORDER BY a.b,a.c,a.as_cls_uid,a.as_subj_uid) AS RowNum
            //	from (
            //	SELECT 
            //			as_id_no = convert(varchar(10),s04_student.std_identity) ,		--身份證字號
            //			as_std_no = convert(varchar(10),s04_student.std_no),			--學號
            //			as_grade = convert(smallint,s04_stuhcls.grd_id) ,				--學生年級
            //			as_cls_uid = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
            //				convert(varchar(1),s04_noropenc.dep_id) + 
            //				case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
            //				convert(varchar(1),s04_noropenc.grd_id) + 
            //				case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
            //			 as_cls_no = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級代碼
            //				convert(varchar(1),s04_noropenc.dep_id) + 
            //				case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
            //				convert(varchar(1),s04_noropenc.grd_id) + 
            //				case when s04_noropenc.cls_id >= 10 then '0' + convert(varchar(2),s04_noropenc.cls_id) else '00' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
            //			as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname), --開課班級名稱
            //			as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end), --開課班級內分組
            //			as_subj_uid = convert(varchar(10),s04_subject.sub_id), --科目內碼
            //			as_course_id = convert(varchar(23),s04_stddbgo.course_code), --課程計畫平臺核發之課程代碼	
            //			as_subj_name = convert(varchar(90),s04_108subject.sub108_name), --科目名稱
            //			as_credits = convert(varchar(10),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end), --學分/時數
            //			as_type = convert(varchar(1),s04_norstusc.stu_status),	--修課類別
            //			as_use_credits = convert(varchar(1),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ), --是否採計學分
            //			as_o_syear =null, --原(應)修課學年度
            //			as_o_sems = null, --原(應)修課學期
            //			as_o_cls_year = null, --原(應)開課年級
            //			as_o_cls_no = null, --原(應)開課班級代碼
            //			as_o_cls_name = null, --原(應)開課班級名稱
            //			as_o_cls_group = null, --原(應)開課班級內分組
            //			as_o_subj_id = null, --原(應)修校務系統科目內碼
            //			as_o_course_id = null , --原(應)修課程代碼
            //			as_o_subj_name = null, --原(應)修科目名稱
            //			as_o_credits = null, --原(應)修學分
            //			as_makeup_mode = null, --補修方式
            //			as_rebuild_mode = null, --重修方式
            //			as_is_precourse = '0', --是否為預選課程
            //			as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid), --授課教師內碼
            //			as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode), --授課教師代碼
            //						as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname), --授課教師姓名
            //			s04_stuhcls.sch_no as a,
            //			s04_stuhcls.year_id as b,
            //			s04_stuhcls.sms_id as c,
            //			d = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
            //				convert(varchar(1),s04_noropenc.dep_id) + 
            //				case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
            //				convert(varchar(1),s04_noropenc.grd_id) + 
            //				case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
            //			e = convert(varchar(10),s04_subject.sub_id),
            //			f = convert(varchar(23),s04_stddbgo.course_code),
            //			g = convert(varchar(50),s04_noropenc.all_empid),
            //			h = s04_student.std_no ,
            //			a.ser_id as i,
            //			s04_noropenc.sub_credit as j,
            //			convert(varchar(1),s04_norstusc.stu_status) as k,
            //			case when isnull(a.attestation_send,'') = '' then '' 
            //			else substring(a.attestation_send,1,3)+'/'+
            //			substring(a.attestation_send,4,2)+'/'+
            //			substring(a.attestation_send,6,2)+' '+
            //			substring(a.attestation_send,8,2)+':'+
            //			substring(a.attestation_send,10,2)+':'+
            //			substring(a.attestation_send,12,2)
            //			end  as l,
            //			case when isnull(a.attestation_release,'') = '' then '' 
            //			else substring(a.attestation_date,1,3)+'/'+
            //			substring(a.attestation_date,4,2)+'/'+
            //			substring(a.attestation_date,6,2)+' '+
            //			substring(a.attestation_date,8,2)+':'+
            //			substring(a.attestation_date,10,2)+':'+
            //			substring(a.attestation_date,12,2)
            //			end  as m,
            //			a.attestation_status as n,
            //			a.is_sys as o,
            //			a.content as p,
            //			convert(varchar(90),s04_108subject.sub108_name) as q,
            //			convert(varchar(50),s04_noropenc.all_empname) as r,
            //			s = 
            //			(
            //			Select SUBSTRING(
            //				(Select ','+ file_name
            //				From L01_std_public_filehub b
            //				join L01_std_attestation_file cc on 
            //				cc.complex_key = b.complex_key and
            //				cc.class_name  = b.class_name and
            //				cc.type_id = b.type_id and
            //				cc.number_id = b.number_id and
            //				cc.check_yn = 'Y'
            //			where 
            //					b.complex_key = s04_stuhcls.sch_no+'_'+
            //									convert(varchar,s04_stuhcls.year_id)+'_'+
            //									convert(varchar,s04_stuhcls.sms_id)+'_'+
            //									convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
            //									convert(varchar(1),s04_noropenc.dep_id) + 
            //									case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
            //									convert(varchar(1),s04_noropenc.grd_id) + 
            //									case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end )+'_'+
            //										convert(varchar(10),s04_subject.sub_id)+'_'+
            //									convert(varchar(23),s04_stddbgo.course_code)+'_'+
            //									convert(varchar(50),s04_noropenc.all_empid)+'_'+
            //									s04_student.std_no+'_0' and
            //					b.class_name = 'StuAttestation' 
            //				For Xml Path(''))
            //			, 2, 8000)
            //			),
            //			u = a.attestation_confirm,
            //			v = a.attestation_centraldb		
            //		FROM s04_student
            //		inner join s04_stuhcls on
            //			s04_student.std_no = s04_stuhcls.std_no and												
            //			s04_stuhcls.sch_no = @sch_no and 
            //			s04_stuhcls.year_id = @year_id and 
            //			s04_stuhcls.sms_id = @sms_id and
            //			s04_student.std_no = @std_no
            //		inner join s90_cha_id on
            //			s04_stuhcls.std_status = s90_cha_id.cha_id and
            //			s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
            //		inner join s04_norstusc on
            //			s04_student.std_no = s04_norstusc.std_no and
            //			s04_norstusc.in_year = s04_stuhcls.year_id and
            //			s04_norstusc.in_sms = s04_stuhcls.sms_id and
            //			s04_norstusc.stu_status = 1 --新修
            //		inner join s04_noropenc on
            //			s04_norstusc.noroc_id = s04_noropenc.noroc_id and
            //			isnull(s04_noropenc.course_code,'') <> '' and
            //			isnull(s04_noropenc.is_learn,'N') = 'Y'
            //		inner join s04_subject on
            //			s04_noropenc.sub_id = s04_subject.sub_id
            //		inner join s04_stddbgo on
            //			s04_stuhcls.year_id - s04_stuhcls.grd_id + 1 = s04_stddbgo.year_id and
            //			s04_stuhcls.sms_id = s04_stddbgo.sms_id and
            //			s04_stuhcls.deg_id = s04_stddbgo.deg_id and
            //			s04_stuhcls.dep_id = s04_stddbgo.dep_id and
            //			s04_stuhcls.bra_id = s04_stddbgo.bra_id and
            //			s04_stuhcls.grd_id = s04_stddbgo.grd_id and
            //			s04_stuhcls.cg_code = s04_stddbgo.cg_code and
            //			isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
            //			s04_noropenc.sub_id = s04_stddbgo.sub_id and
            //			( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
            //		inner join s04_108subject on
            //			s04_stddbgo.course_code = s04_108subject.course_code
            //		inner join L01_std_attestation a on
            //			a.year_id = s04_stuhcls.year_id and
            //			a.sms_id = s04_stuhcls.sms_id and
            //			a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
            //					convert(varchar(1),s04_noropenc.dep_id) + 
            //					case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
            //					convert(varchar(1),s04_noropenc.grd_id) + 
            //					case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ) and
            //			a.sub_id = convert(varchar(10),s04_subject.sub_id) and
            //			a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
            //			a.emp_id =  convert(varchar(50),s04_noropenc.all_empid) and 
            //			isnull(a.attestation_send,'') <>'' and
            //			--isnull(a.attestation_confirm,'') = 'Y' and  
            //			isnull(a.attestation_status,'') = 'Y'and
            //			isnull(a.attestation_release,'') = 'Y'and
            //			a.std_no = s04_student.std_no
            //		left join s90_organization on
            //			s04_noropenc.org_id = s90_organization.org_id
            //		left join s90_branch on
            //			s04_noropenc.bra_id = s90_branch.bra_id
            //		left join s04_hstusubjscoreterm on
            //			s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
            //			s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
            //			s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
            //			s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
            //			s04_hstusubjscoreterm.hstusst_status = 1
            //		left join s04_hstusixscoreterm on
            //			s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
            //			s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
            //			s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
            //		left join s04_stubuterm on
            //			s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
            //			s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
            //			s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
            //			s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
            //		where s04_student.std_hisdatayear >= 108
            //	union
            //	SELECT 
            //			as_id_no = convert(varchar(10),s04_student.std_identity) ,
            //			as_std_no = convert(varchar(10),s04_student.std_no),
            //			as_grade = convert(smallint,s04_stuhcls.grd_id) ,
            //			as_cls_uid = convert(varchar(6),s04_noropenc.noroc_id),
            //			as_cls_no = convert(varchar(6),s04_noropenc.noroc_id),
            //			as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname),
            //			as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end),
            //			as_subj_uid = convert(varchar(10),s04_subject.sub_id),
            //			as_course_id = convert(varchar(23),s04_stddbgo.course_code),
            //			as_subj_name = convert(varchar(90),s04_108subject.sub108_name),
            //			as_credits = convert(varchar(1),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
            //			as_type = convert(varchar(1),s04_norstusc.stu_status),
            //			as_use_credits = convert(varchar(10),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ),
            //			as_o_syear = convert(smallint , s04_hstusubjscoreterm.in_year ),
            //			as_o_seme = convert(smallint , s04_hstusubjscoreterm.in_sms ),
            //			as_o_cls_year = convert(varchar(1) , noro_o.grd_id ),
            //			as_o_cls_no = convert(varchar(1),noro_o.dep_id) + 
            //				case when noro_o.bra_id >= 10 then convert(varchar(2),noro_o.bra_id) else '0' + convert(varchar(1),noro_o.bra_id) end + 
            //				convert(varchar(1),noro_o.grd_id) + 
            //				case when noro_o.cls_id >= 100 then convert(varchar(3),noro_o.cls_id) when noro_o.cls_id >= 10 then '0' + convert(varchar(2),noro_o.cls_id) else '00' + convert(varchar(1),noro_o.cls_id) end,
            //			 as_o_cls_name = convert(varchar(90),noro_o.noroc_clsname),
            //			as_o_cls_group = convert(varchar(20),case when isnull(noro_o.org_id,0) in (0,99) then '無' else org_o.org_name end) ,
            //			as_o_subj_id = convert(varchar(10),noro_o.sub_id),
            //			as_o_course_id = convert(varchar(23),noro_o.course_code),
            //			as_o_subj_name = convert(varchar(90),s04_108subject.sub108_name),
            //			as_o_credits = convert(smallint , s04_noropenc.sub_credit) ,
            //			as_makeup_mode = null,
            //			as_rebuild_mode = convert(varchar(1),case isnull(s04_hstusubjscoreterm.hstusst_outsch,'0') when '1' then 4 when '2' then 5 when '3' then 6 end),
            //			as_is_precourse = '0',
            //			as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid),
            //			as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode),
            //						as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
            //			s04_stuhcls.sch_no as a,
            //			s04_stuhcls.year_id as b,
            //			s04_stuhcls.sms_id as c,
            //			d = convert(varchar(6),s04_noropenc.noroc_id),
            //			e = convert(varchar(10),s04_subject.sub_id),
            //			f = convert(varchar(23),s04_stddbgo.course_code),
            //			g = convert(varchar(50),s04_noropenc.all_empid),
            //			h = s04_student.std_no ,
            //			a.ser_id as i,
            //			s04_noropenc.sub_credit as j,
            //			convert(varchar(1),s04_norstusc.stu_status) as k,
            //			case when isnull(a.attestation_send,'') = '' then '' 
            //			else substring(a.attestation_send,1,3)+'/'+
            //			substring(a.attestation_send,4,2)+'/'+
            //			substring(a.attestation_send,6,2)+' '+
            //			substring(a.attestation_send,8,2)+':'+
            //			substring(a.attestation_send,10,2)+':'+
            //			substring(a.attestation_send,12,2)
            //			end  as l,
            //			case when isnull(a.attestation_release,'') = '' then '' 
            //			else substring(a.attestation_date,1,3)+'/'+
            //			substring(a.attestation_date,4,2)+'/'+
            //			substring(a.attestation_date,6,2)+' '+
            //			substring(a.attestation_date,8,2)+':'+
            //			substring(a.attestation_date,10,2)+':'+
            //			substring(a.attestation_date,12,2)
            //			end  as m,
            //			a.attestation_status as n,
            //			a.is_sys as o,
            //			a.content as p,
            //			convert(varchar(90),s04_108subject.sub108_name) as q,
            //			convert(varchar(50),s04_noropenc.all_empname) as r,
            //			s = 
            //			(
            //			Select SUBSTRING(
            //				(Select ','+ file_name
            //				From L01_std_public_filehub b
            //				join L01_std_attestation_file cc on 
            //				cc.complex_key = b.complex_key and
            //				cc.class_name  = b.class_name and
            //				cc.type_id = b.type_id and
            //				cc.number_id = b.number_id and
            //				cc.check_yn = 'Y'
            //			where 
            //				b.complex_key = s04_stuhcls.sch_no+'_'+
            //								convert(varchar,s04_stuhcls.year_id)+'_'+
            //								convert(varchar,s04_stuhcls.sms_id)+'_'+
            //								convert(varchar(6),s04_noropenc.noroc_id)+'_'+
            //								convert(varchar(10),s04_subject.sub_id)+'_'+
            //								convert(varchar(23),s04_stddbgo.course_code)+'_'+
            //								convert(varchar(50),s04_noropenc.all_empid)+'_'+
            //								s04_student.std_no+'_0' and
            //					b.class_name = 'StuAttestation' 
            //				For Xml Path(''))
            //			, 2, 8000)
            //			),
            //			u = a.attestation_confirm,
            //			v = a.attestation_centraldb			
            //		FROM s04_student
            //		inner join s04_stuhcls on
            //			s04_student.std_no = s04_stuhcls.std_no and												
            //			s04_stuhcls.sch_no = @sch_no and 
            //			s04_stuhcls.year_id = @year_id and 
            //			s04_stuhcls.sms_id = @sms_id and
            //			s04_student.std_no = @std_no
            //		inner join s90_cha_id on
            //			s04_stuhcls.std_status = s90_cha_id.cha_id and
            //			s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370') --只上傳正常學籍、休學、輔導延修
            //		inner join s04_noropenc on
            //			s04_noropenc.year_id = s04_stuhcls.year_id and
            //			s04_noropenc.sms_id = s04_stuhcls.sms_id
            //		inner join s04_norstusc on
            //			s04_norstusc.noroc_id = s04_noropenc.noroc_id and
            //			s04_student.std_no = s04_norstusc.std_no and
            //			s04_norstusc.stu_status = 2 --重修
            //		inner join s04_subject on
            //			s04_noropenc.sub_id = s04_subject.sub_id
            //		inner join s04_hstusubjscoreterm on
            //			s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
            //			s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
            //			s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
            //			s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
            //			s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
            //			s04_hstusubjscoreterm.hstusst_status = 2
            //		inner join s04_stddbgo on
            //			s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
            //			s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
            //			s04_stuhcls.deg_id = s04_stddbgo.deg_id and
            //			s04_stuhcls.dep_id = s04_stddbgo.dep_id and
            //			s04_stuhcls.bra_id = s04_stddbgo.bra_id and
            //			s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
            //			s04_stuhcls.cg_code = s04_stddbgo.cg_code and
            //			isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
            //			s04_noropenc.sub_id = s04_stddbgo.sub_id and
            //			( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
            //		inner join s04_108subject on
            //			s04_stddbgo.course_code = s04_108subject.course_code
            //		inner join s90_organization on
            //			s04_noropenc.org_id = s90_organization.org_id
            //		inner join s90_branch on
            //			s04_noropenc.bra_id = s90_branch.bra_id
            //		inner join s04_hstusixscoreterm on
            //			s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
            //			s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
            //			s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
            //		 join L01_std_attestation a on
            //			a.year_id = s04_stuhcls.year_id and
            //			a.sms_id = s04_stuhcls.sms_id and
            //			a.cls_id = convert(varchar(6),s04_noropenc.noroc_id) and
            //			a.sub_id = convert(varchar(10),s04_subject.sub_id) and
            //			a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
            //			a.emp_id =  convert(varchar(50),s04_noropenc.all_empid) and 
            //			isnull(a.attestation_send,'') <>'' and 
            //			--isnull(a.attestation_confirm,'') = 'Y' and  
            //			isnull(a.attestation_status,'') = 'Y' and
            //			isnull(a.attestation_release,'') = 'Y'and
            //			a.std_no = s04_student.std_no
            //		left join s04_stubuterm on
            //			s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
            //			s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
            //			s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
            //			s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id 
            //		inner join s04_hstusubjscoreterm hstusst_new on
            //			s04_hstusubjscoreterm.sub_id = hstusst_new.sub_id and
            //			s04_hstusubjscoreterm.std_no = hstusst_new.std_no and
            //			s04_hstusubjscoreterm.in_year = hstusst_new.in_year and
            //			s04_hstusubjscoreterm.in_sms = hstusst_new.in_sms and
            //			hstusst_new.hstusst_status = 1
            //		inner join s04_norstusc norstu_o on
            //			hstusst_new.std_no = norstu_o.std_no and
            //			hstusst_new.in_year = norstu_o.in_year and
            //			hstusst_new.in_sms = norstu_o.in_sms and
            //			norstu_o.stu_status = 1
            //		inner join s04_noropenc noro_o on
            //			norstu_o.noroc_id = noro_o.noroc_id and
            //			hstusst_new.sub_id = noro_o.sub_id 		
            //		inner join s90_organization org_o on
            //			noro_o.org_id = org_o.org_id
            //		where s04_student.std_hisdatayear >= 108 
            //	union
            //	SELECT 
            //			as_id_no = convert(varchar(10),s04_student.std_identity) ,
            //			as_std_no = convert(varchar(10),s04_student.std_no),
            //			as_grade = convert(smallint,s04_stuhcls.grd_id) ,
            //			as_cls_uid = convert(varchar(6),s04_noropenc.noroc_id),
            //			as_cls_no = convert(varchar(6),s04_noropenc.noroc_id),
            //			as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname),
            //			as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end),
            //			as_subj_uid = convert(varchar(10),s04_subject.sub_id),
            //			as_course_id = convert(varchar(23),s04_stddbgo.course_code),
            //			as_subj_name = convert(varchar(90),s04_108subject.sub108_name),
            //			as_credits = convert(varchar(1),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
            //			as_type = convert(varchar(1),s04_norstusc.stu_status),
            //			as_use_credits = convert(varchar(10),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ),
            //			as_o_syear = convert(smallint , s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 ),
            //			as_o_sems = convert(smallint , s04_stddbgo.sms_id ),
            //			as_o_cls_year = convert(varchar(1) , s04_stddbgo.grd_id ),
            //			as_o_cls_no = convert(varchar(1),s04_stddbgo.dep_id) + 
            //				case when s04_stddbgo.bra_id >= 10 then convert(varchar(2),s04_stddbgo.bra_id) else '0' + convert(varchar(1),s04_stddbgo.bra_id) end + 
            //				convert(varchar(1),s04_stddbgo.grd_id) + 
            //				case when s04_stuhcls.cls_id >= 10 then '0' + convert(varchar(2),s04_stuhcls.cls_id) else '00' + convert(varchar(1),s04_stuhcls.cls_id) end,
            //			as_o_cls_name = convert(varchar(60),s04_ytdbgoc.cls_abr),
            //			as_o_cls_group = convert(varchar(20),case when isnull(s04_ytdbgoc.org_id,0) in (0,99) then '無' else org_o.org_name end) ,
            //			as_o_subj_id = convert(varchar(10),s04_stddbgo.sub_id),
            //			as_o_course_id = convert(varchar(23),s04_stddbgo.course_code),
            //			as_o_subj_name = convert(varchar(90),subj108_o.sub108_name),
            //			as_o_credits = convert(smallint , s04_stddbgo.sub_credit) ,
            //			as_makeup_mode = convert(varchar(1),case isnull(s04_hstusubjscoreterm.hstusst_outsch,'0') when '1' then 4 when '2' then 5 when '3' then 6 end),
            //			as_rebuild_mode = null,
            //			as_is_precourse = '0',
            //			as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid),
            //			as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode),
            //						as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
            //			s04_stuhcls.sch_no as a,
            //			s04_stuhcls.year_id as b,
            //			s04_stuhcls.sms_id as c,
            //			d = convert(varchar(6),s04_noropenc.noroc_id),
            //			e = convert(varchar(10),s04_subject.sub_id),
            //			f = convert(varchar(23),s04_stddbgo.course_code),
            //			g = convert(varchar(50),s04_noropenc.all_empid),
            //			h = s04_student.std_no ,
            //			a.ser_id as i,
            //			s04_noropenc.sub_credit as j,
            //			convert(varchar(1),s04_norstusc.stu_status) as k,
            //			case when isnull(a.attestation_send,'') = '' then '' 
            //			else substring(a.attestation_send,1,3)+'/'+
            //			substring(a.attestation_send,4,2)+'/'+
            //			substring(a.attestation_send,6,2)+' '+
            //			substring(a.attestation_send,8,2)+':'+
            //			substring(a.attestation_send,10,2)+':'+
            //			substring(a.attestation_send,12,2)
            //			end  as l,
            //			case when isnull(a.attestation_release,'') = '' then '' 
            //			else substring(a.attestation_date,1,3)+'/'+
            //			substring(a.attestation_date,4,2)+'/'+
            //			substring(a.attestation_date,6,2)+' '+
            //			substring(a.attestation_date,8,2)+':'+
            //			substring(a.attestation_date,10,2)+':'+
            //			substring(a.attestation_date,12,2)
            //			end  as m,
            //			a.attestation_status as n,
            //			a.is_sys as o,
            //			a.content as p,
            //			convert(varchar(90),s04_108subject.sub108_name) as q,
            //			convert(varchar(50),s04_noropenc.all_empname) as r,
            //			s = 
            //			(
            //			Select SUBSTRING(
            //				(Select ','+ file_name
            //				From L01_std_public_filehub b
            //				join L01_std_attestation_file cc on 
            //				cc.complex_key = b.complex_key and
            //				cc.class_name  = b.class_name and
            //				cc.type_id = b.type_id and
            //				cc.number_id = b.number_id and
            //				cc.check_yn = 'Y'
            //			where 
            //				b.complex_key = s04_stuhcls.sch_no+'_'+
            //								convert(varchar,s04_stuhcls.year_id)+'_'+
            //								convert(varchar,s04_stuhcls.sms_id)+'_'+
            //								convert(varchar(6),s04_noropenc.noroc_id)+'_'+
            //								convert(varchar(10),s04_subject.sub_id)+'_'+
            //								convert(varchar(23),s04_stddbgo.course_code)+'_'+
            //								convert(varchar(50),s04_noropenc.all_empid)+'_'+
            //								s04_student.std_no+'_0' and
            //					b.class_name = 'StuAttestation' 
            //				For Xml Path(''))
            //			, 2, 8000)
            //			),
            //			u = a.attestation_confirm,
            //			v = a.attestation_centraldb			
            //		FROM s04_student
            //		inner join s04_stuhcls on
            //			s04_student.std_no = s04_stuhcls.std_no and												
            //			s04_stuhcls.sch_no = @sch_no and 
            //			s04_stuhcls.year_id = @year_id and 
            //			s04_stuhcls.sms_id = @sms_id and
            //			s04_student.std_no = @std_no
            //		inner join s90_cha_id on
            //			s04_stuhcls.std_status = s90_cha_id.cha_id and
            //			s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
            //		inner join s04_noropenc on
            //			s04_noropenc.year_id = s04_stuhcls.year_id and
            //			s04_noropenc.sms_id = s04_stuhcls.sms_id 
            //		inner join s04_norstusc on
            //			s04_norstusc.noroc_id = s04_noropenc.noroc_id and
            //			s04_student.std_no = s04_norstusc.std_no and
            //			s04_norstusc.stu_status = 3 --補修
            //		inner join s04_subject on
            //			s04_noropenc.sub_id = s04_subject.sub_id
            //		inner join s90_organization on
            //			s04_noropenc.org_id = s90_organization.org_id
            //		inner join s04_hstusubjscoreterm on
            //			s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
            //			s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
            //			s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
            //			s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
            //			s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
            //			s04_hstusubjscoreterm.hstusst_status = 3
            //		left join s04_hstusixscoreterm on
            //			s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
            //			s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
            //			s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
            //		left join s04_stubuterm on
            //			s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
            //			s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
            //			s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
            //			s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
            //		inner join s04_stddbgo on
            //			s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
            //			s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
            //			s04_stuhcls.deg_id = s04_stddbgo.deg_id and
            //			s04_stuhcls.dep_id = s04_stddbgo.dep_id and
            //			s04_stuhcls.bra_id = s04_stddbgo.bra_id and
            //			s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
            //			s04_stuhcls.cg_code = s04_stddbgo.cg_code and
            //			isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
            //			s04_noropenc.sub_id = s04_stddbgo.sub_id and
            //			( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
            //		inner join s04_108subject on
            //			s04_stddbgo.course_code = s04_108subject.course_code
            //		inner join s04_ytdbgoc on
            //			s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 = s04_ytdbgoc.year_id and
            //			s04_stddbgo.sms_id = s04_ytdbgoc.sms_id and
            //			s04_stddbgo.dep_id = s04_ytdbgoc.dep_id and
            //			s04_stddbgo.deg_id = s04_ytdbgoc.deg_id and
            //			s04_stddbgo.bra_id = s04_ytdbgoc.bra_id and
            //			s04_stddbgo.grd_id = s04_ytdbgoc.grd_id and
            //			s04_stuhcls.cls_id = s04_ytdbgoc.cls_id
            //		inner join s90_organization org_o on
            //			s04_ytdbgoc.org_id = org_o.org_id
            //		inner join s90_branch on
            //			s04_ytdbgoc.bra_id = s90_branch.bra_id
            //		inner join s04_108subject subj108_o on
            //			s04_stddbgo.course_code = subj108_o.course_code
            //		 join L01_std_attestation a on
            //			a.year_id = s04_stuhcls.year_id and
            //			a.sms_id = s04_stuhcls.sms_id and
            //			a.cls_id = convert(varchar(6),s04_noropenc.noroc_id) and
            //			a.sub_id = convert(varchar(10),s04_subject.sub_id) and
            //			a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
            //			a.emp_id =  convert(varchar(50),s04_noropenc.all_empid) and
            //			isnull(a.attestation_send,'') <>'' and
            //			--isnull(a.attestation_confirm,'') = 'Y' and 
            //			isnull(a.attestation_status,'') = 'Y' and
            //			isnull(a.attestation_release,'') = 'Y'and
            //			a.std_no =s04_student.std_no
            //		where s04_student.std_hisdatayear >= 108
            //		) a
            //                           ) AS NewTable
            //                       WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun 
            //                       order by as_cls_uid,as_course_id";
            #endregion

            string str_sql = @"
											SELECT 
											*,
											x_cnt = 
												(select count(*)
												 from L01_std_public_filehub a
												 join L01_std_attestation_file b on 
																						a.complex_key = b.complex_key and
																						a.number_id = b.number_id and
																						a.type_id = b.type_id and
																						a.class_name = b.class_name and
																						b.check_yn = 'Y'
												 where a.class_name = 'StuAttestation'
												 and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0'
												 and a.number_id = NewTable.i),
											x_total =
											(select count(*)
											from L01_std_attestation a
											where a.year_id = NewTable.b
											and   a.sms_id = NewTable.c
											and   a.std_no = NewTable.h
											and isnull(a.attestation_send,'') <>'' 
											and isnull(a.attestation_status,'') = 'Y')
											FROM(
											select *,
												   ROW_NUMBER() OVER(ORDER BY a.b,a.c,a.d,a.e) AS RowNum
											from (
														select 
														a = sch_no,
														b = year_id,
														c = sms_id ,
														d = cls_id,
														e = sub_id,
														f = src_dup,
														g = emp_id,
														h = std_no,
														i = ser_id,
														j = credit,
														k = stu_status,
														l = case 
															when isnull(attestation_send,'') = '' then '' 
															else	substring(attestation_send,1,3)+'/'+
																	substring(attestation_send,4,2)+'/'+
																	substring(attestation_send,6,2)+' '+
																	substring(attestation_send,8,2)+':'+
																	substring(attestation_send,10,2)+':'+
																	substring(attestation_send,12,2)
															end,
														m = case
															when isnull(attestation_release,'') = '' then '' 
															else	substring(attestation_date,1,3)+'/'+
																	substring(attestation_date,4,2)+'/'+
																	substring(attestation_date,6,2)+' '+
																	substring(attestation_date,8,2)+':'+
																	substring(attestation_date,10,2)+':'+
																	substring(attestation_date,12,2)
															end,
														n = attestation_status,
														o = is_sys,
														p = content,
														q = sub_name,
														r = all_empname,
														s = 
														(
														Select SUBSTRING(
															(Select ','+ file_name
															 From L01_std_public_filehub b
															 join  L01_std_attestation_file cc on 
																									cc.complex_key = b.complex_key and
																									cc.class_name  = b.class_name and
																									cc.type_id = b.type_id and
																									cc.number_id = b.number_id and
																									cc.check_yn = 'Y'
															 where 
																b.complex_key = L01_std_attestation.sch_no+'_'+
																				convert(varchar,L01_std_attestation.year_id)+'_'+
																				convert(varchar,L01_std_attestation.sms_id)+'_'+
																				L01_std_attestation.cls_id+'_'+
																				L01_std_attestation.sub_id+'_'+
																				L01_std_attestation.src_dup+'_'+
																				L01_std_attestation.emp_id+'_'+
																				L01_std_attestation.std_no+'_0' and
																b.number_id = L01_std_attestation.ser_id and
																b.class_name = 'StuAttestation' 
															For Xml Path(''))
														, 2, 8000)
														),
														w = case isnull(attestation_reason,'')
															when '01' then '非本課程相關主題成果，無法認證'
															when '02' then '無法查證成果可信度'
															when '03' then '請加強成果內容正確性'
															when '04' then '請增加成果內容豐富度'  else '' end,
														x = attestation_release,
														u = attestation_confirm,
														v = attestation_centraldb
														from L01_std_attestation
														join L01_std_attestation_file a on a.complex_key = 
														L01_std_attestation.sch_no+'_'+convert(varchar,L01_std_attestation.year_id)+'_'+convert(varchar,L01_std_attestation.sms_id)+'_'+
														L01_std_attestation.cls_id+'_'+L01_std_attestation.sub_id+'_'+L01_std_attestation.src_dup+'_'+L01_std_attestation.emp_id+'_'+
														L01_std_attestation.std_no+'_0' and
														a.number_id = L01_std_attestation.ser_id and
														a.check_yn = 'Y' and
														a.class_name = 'StuAttestation'
														where 										
														sch_no = @sch_no and 
														year_id = @year_id and 
														sms_id = @sms_id and
														std_no = @std_no and	
														isnull(attestation_send,'') <>'' and  
														isnull(attestation_status,'') = 'Y'and
														isnull(attestation_release,'') = 'Y'
														)a
											)as NewTable
											where RowNum >= @sRowNun AND RowNum <= @eRowNun
											order by
											d,e";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<HeaderList>(str_sql, arg);
            }
        }
        public async Task<IEnumerable<L01_StuAttestation>> GetFormData(StuAttestationQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT *
                                FROM L01_stu_group
                                WHERE sch_no = @sch_no 
                                and year_id = @year_id 
                                and sms_id = @sms_id 
                                and std_no = @std_no
                                and ser_id = @ser_id
                            ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<L01_StuAttestation>(str_sql, arg);
            }
        }
        public async Task<int> DeleteData(StuAttestationQueryList arg)
        {
			try 
			{
                arg.sch_no = _context.SchNo;
                string str_sql =string.Format( @"
                                delete                                
                                FROM L01_std_attestation
                                WHERE sch_no+'_'+convert(varchar,year_id)+'_'+convert(varchar,sms_id)+'_'+cls_id+'_'+sub_id+'_'+src_dup+'_'+emp_id+'_'+std_no+'_' +convert(varchar,ser_id) = '{0}'
                            ",arg.argdata);
                using (IDbConnection conn = _context.CreateCommand())
                {
                    return await conn.ExecuteAsync(str_sql, arg);
                }
            }
			catch (Exception ex) 
			{
				return 0;
			}
           
        }
        public async Task<int> UpdateData(L01_StuAttestation arg)
        {
            arg.sch_no = _context.SchNo;
            arg.upd_dt = updteDate();
            arg.upd_name = arg.std_no;

            string str_sql = @"
                                update L01_std_attestation
                                set 
                                    attestation_send =  @upd_dt,
                                    attestation_status = 'N',
									attestation_reason = '',
									attestation_release = ''
                                WHERE sch_no = @sch_no 
                                and year_id = @year_id 
                                and sms_id = @sms_id 
                                and cls_id = @cls_id
                                and sub_id = @sub_id
                                and src_dup = @src_dup
                                and emp_id = @emp_id
                                and std_no = @std_no
								and ser_id = @ser_id
                            ";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(str_sql, arg);
            }
        }
        public async Task<int> UpdateDataConfirm(L01_StuAttestationList arg)
        {
            //arg.sch_no = _schno;
            //arg.upd_dt = updteDate();
            //arg.upd_name = arg.std_no;

            string str_sql = @"
                                update L01_std_attestation
                                set 
                                    attestation_confirm =  'Y'
                                WHERE sch_no +'_'+convert(varchar,year_id)+'_'+convert(varchar,sms_id)+'_'+cls_id+'_'+sub_id+'_'+src_dup+'_'+emp_id+'_'+std_no+'_'+convert(varchar,ser_id) in @complex_key
                            ";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(str_sql, arg);
            }
        }
        public async Task<int> UpdateDataCentraldb(L01_StuAttestationList arg)
        {
            //arg.sch_no = _schno;
            //arg.upd_dt = updteDate();
            //arg.upd_name = arg.std_no;

            var dynamicParams = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int i = 1;
            int j = 1;
            foreach (var item in arg.complex_key)
			{
                sb.Append(string.Format(@"
                                update L01_std_attestation
                                set 
                                    attestation_centraldb = case when @arg2_{0} ='true' then 'Y' else 'N' end
                                WHERE sch_no +'_'+convert(varchar,year_id)+'_'+convert(varchar,sms_id)+'_'+cls_id+'_'+sub_id+'_'+src_dup+'_'+emp_id+'_'+std_no+'_'+convert(varchar,ser_id) = @arg1_{1}
                            ",i,j));

				dynamicParams.Add("arg1_"+i, item.Split('@')[0]);
                dynamicParams.Add("arg2_"+j, item.Split('@')[1]);
				i++;
				j++;
            };
            //string str_sql = @"
            //                    update L01_std_attestation
            //                    set 
            //                        attestation_centraldb =  'Y'
            //                    WHERE sch_no +'_'+convert(varchar,year_id)+'_'+convert(varchar,sms_id)+'_'+cls_id+'_'+sub_id+'_'+src_dup+'_'+emp_id+'_'+std_no in @complex_key
            //                ";
     
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(sb.ToString(), dynamicParams);
                //return await conn.ExecuteAsync(str_sql, arg);
            }
        }
        public async Task<Dictionary<string, string>> InsertData(L01_StuAttestation arg)
        {
            arg.sch_no = _context.SchNo;
            arg.upd_dt = updteDate();
            arg.upd_name = arg.std_no;
            string str_sql = "";
            var dict = new Dictionary<string, string>();
            str_sql = @"select * 
                        from L01_stu_group 
                        where year_id = @year_id
                        and sms_id = @sms_id
                        and std_no = @std_no
                        and sch_no = @sch_no
                        ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                var dr = await conn.ExecuteReaderAsync(str_sql, arg);
                DataTable dt = new DataTable();
                dt.Load(dr);
                /*
                var result = (from r in dt.AsEnumerable()
                              where
                                   r.Field<string>("time_id") == arg.time_id &&
                                    r.Field<string>("startdate") == arg.startdate &&
                                    r.Field<string>("enddate") == arg.enddate
                              select r).Count();
                */
                var result = 0;
                if (result > 0)
                {
                    dict.Add("haveData", "Y");
                    dict.Add("result", "0");
                    return dict;
                }
                else
                {
                    var num = (from r in dt.AsEnumerable()
                               where r.Field<Int16>("year_id") == arg.year_id &&
                                     r.Field<Int16>("sms_id") == arg.sms_id &&
                                     r.Field<string>("std_no") == arg.std_no &&
                                     r.Field<string>("sch_no") == arg.sch_no
                               select r).Count();

                    if (num > 0)
                    {
                        num = (from r in dt.AsEnumerable()
                               where r.Field<Int16>("year_id") == arg.year_id &&
                                     r.Field<Int16>("sms_id") == arg.sms_id &&
                                     r.Field<string>("std_no") == arg.std_no &&
                                     r.Field<string>("sch_no") == arg.sch_no
                               select r).Max(x => x.Field<Int16>("ser_id")) + 1;
                    }
                    else 
                    {
                        num = 1;
                    }
                    arg.ser_id = num;
                    str_sql = @"
                            insert L01_stu_group
                            (sch_no,year_id,sms_id,ser_id,std_no,time_id,unit_name,group_content,startdate,
                            enddate,hours,content,is_sys,upd_name,upd_dt)
                            values
                            (@sch_no,@year_id,@sms_id,@ser_id,@std_no,@time_id,@unit_name,@group_content,@startdate,
                            @enddate,@hours,@content,@is_sys,@upd_name,@upd_dt)
                        ";
                    var result_count = await conn.ExecuteAsync(str_sql, arg);
                    dict.Add("haveData", "Y");
                    dict.Add("result", result_count.ToString());
                    
                    return dict;
                }
            }
        }
        public async Task<int> UpdateDataCheckYN(L01_StuAttestationCheck arg)
        {
			//arg.sch_no = _schno;
			//arg.upd_dt = updteDate();
			//arg.upd_name = arg.std_no;

			string str_sql = @"
											update L01_std_attestation_file
											set check_yn = @check_yn
											where complex_key = @complex_key
											and class_name = 'StuAttestation'
											and type_id = 0
											and number_id = @number_id
											";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(str_sql, arg);
            }
        }
    }
}
