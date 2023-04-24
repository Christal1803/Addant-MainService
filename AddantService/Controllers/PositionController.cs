using AddantSDAL.DAL;
using AddantSDAL.DTO;
using AddantService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace AddantService.Controllers
{
	[RoutePrefix("api/Position")]
	public class PositionController : BaseController
	{
		IPositionRepository _positionRepository;

		public PositionController(IPositionRepository positionRepository)
		{
			_positionRepository = positionRepository;
		}
		/// <summary>
		/// get all position
		/// </summary>
		/// <returns></returns>

		///// <summary>
		/// get a specific position detail
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		/// 


		[AllowAnonymous]
		[Route("")]
		public IHttpActionResult GetAllPostion(bool isAdminCall, DateTime? startDate = null, DateTime? endDate = null, string SearchText = "")
		{
			try
			{
				Logger.WriteLog("Inside GetAllPostion()");
				var res = _positionRepository.GetAllPostion(isAdminCall, startDate, endDate, SearchText);
				return WebResult(res);
			}
			catch (Exception ex) { Logger.WriteLog(ex.Message); return null; }
		}
		/// <summary>



		[AllowAnonymous]
		[Route("{id}")]
		public IHttpActionResult GetPositionById(int id)

		{
			try
			{
				var res = _positionRepository.GetPositionById(id);
				return WebResult(res);
			}
			catch (Exception ex) { Logger.WriteLog(ex.Message); return null; }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="positionModel"></param>
		/// <returns></returns>
		 [AllowAnonymous]
		[Route("Compose")]
		public IHttpActionResult ComposePosition([FromBody] Models.PositionModel positionModel)
		{
			try
			{
				PositionDTO _data = new PositionDTO
				{
					CreatedDate = positionModel.CreatedDate,
					IdPosition = positionModel.IdPosition,
					IsDeleted = positionModel.IsDeleted,
					Name = positionModel.Name,
					//New field added to Position table 
					JobId = positionModel.JobId,
					JobStatus = (JobStatusEnum)positionModel.JobStatus,
					Experience = positionModel.Experience,
					ClosingDate = positionModel.ClosingDate,
					Location = positionModel.Location,
					ReportsTo = positionModel.ReportsTo,
					AboutCompany = positionModel.AboutCompany,
					JobOverview = positionModel.JobOverview,
					KeyResponsibility = positionModel.KeyResponsibility,
					Qualification = positionModel.Qualification,
                    Deleted = false,

                    positionDetailDTOs = positionModel.positionDetails?.Select(x => new PositionDetailDTO
					{
						IdPositionDetail = x.IdPositionDetail,
						CreatedDate = x.CreatedDate,
						IdPosition = x.IdPosition,
						IsDeleted = x.IsDeleted,
						SubHeader = x.SubHeader,
						SubHeaderContent = x.SubHeaderContent
					}).ToList()
				};
				var res = _positionRepository.ComposePosition(_data);
				return WebResult(res);
                    Logger.WriteLog("Active" );

            }
            catch (Exception ex) { Logger.WriteLog(ex.Message); return null; }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="idPosition"></param>
		/// <returns></returns>
		[Authorize]
		[Route("Delete")]
		[HttpDelete]
		public IHttpActionResult Delete(int idPosition, bool isDeleted = false)
		{
			try
			{
				var res = _positionRepository.DeletePosition(idPosition, isDeleted);
				return WebResult(res);
			}
			catch (Exception ex) { Logger.WriteLog(ex.Message); return null; }
		}
		/// <summary>
		/// Delete Position and details permanently from Table 
		/// </summary>
		/// <param name="idPosition"></param>
		/// <returns></returns>

		[Authorize]
		[Route("Delete")]
		[HttpDelete]
		public IHttpActionResult Delete(int idPosition)
		{
			try
			{
				var res = _positionRepository.DeletePosition(idPosition);
				return WebResult(res);
			}
			catch (Exception ex) { Logger.WriteLog(ex.Message); return null; }
		}


		[AllowAnonymous]
		[HttpGet]
		[Route("ApplicationBucket")]
		public IHttpActionResult GetApplicationBucket()
		{
			try
			{
				var res = _positionRepository.ApplicationBucket();
				return WebResult(res);
			}
			catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
		}



		////
		///Setting Status Change
		///1-hidden 
		///2-Active
		///3-closed
		///

		[AllowAnonymous]
		[Route("ChangeStatus")]
		[HttpPost]
		public IHttpActionResult ChangeStatus([FromBody] Models.StatusModel statModel)
		{
			var res = _positionRepository.ChangeStatus(statModel.idPosition, statModel.Status);
			return WebResult(res);
		}


        [AllowAnonymous]
        [Route("Deleted/{IdPosition}")]
        [HttpPost]
        public IHttpActionResult DeletedStatus(int IdPosition, bool isDeleted)
        {
            try
            {
                var res = _positionRepository.UpdateDeletedStatus(IdPosition, isDeleted);
                return WebResult(res);
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

    }
}