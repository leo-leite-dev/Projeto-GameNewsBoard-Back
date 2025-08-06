using AutoMapper;
using GameNewsBoard.Application.Commons;
using GameNewsBoard.Application.DTOs.Requests;
using GameNewsBoard.Application.DTOs.Responses.Game;
using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Application.IServices.IGame;
using GameNewsBoard.Domain.Commons;
using GameNewsBoard.Domain.Entities;

namespace GameNewsBoard.Infrastructure.Services
{
    public class GameStatusService : IGameStatusService
    {
        private readonly IGameStatusRepository _gameStatusRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IMapper _mapper;

        public GameStatusService(
            IGameStatusRepository gameStatusRepository,
            IGameRepository gameRepository,
            IMapper mapper)
        {
            _gameStatusRepository = gameStatusRepository ?? throw new ArgumentNullException(nameof(gameStatusRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> SetGameStatusAsync(Guid userId, int gameId, GameStatusRequest request)
        {
            return await ServiceExecutionHelper.TryExecuteAsync(async () =>
            {
                var game = await _gameRepository.GetByIdAsync(gameId)
                           ?? throw new ArgumentException("Jogo não encontrado.");

                var existing = await _gameStatusRepository.GetByUserAndGameIdAsync(userId, gameId);

                if (existing is null)
                {
                    var gameStatus = GameStatus.Create(userId, gameId, request.Status);
                    await _gameStatusRepository.AddAsync(gameStatus);
                }
                else
                {
                    existing.UpdateStatus(request.Status);
                }

                await _gameStatusRepository.SaveChangesAsync();
            }, "Erro ao definir status para o jogo. Tente novamente.");
        }

        public async Task<Result> RemoveGameStatusAsync(Guid userId, int gameId)
        {
            return await ServiceExecutionHelper.TryExecuteAsync(async () =>
            {
                var gameStatus = await _gameStatusRepository.GetByUserAndGameIdAsync(userId, gameId)
                             ?? throw new ArgumentException("Status não encontrado.");

                await _gameStatusRepository.DeleteAsync(gameStatus);
                await _gameStatusRepository.SaveChangesAsync();
            }, "Erro ao remover status do jogo. Tente novamente.");
        }

        public async Task<Result<IEnumerable<GameStatusResponse>>> GetUserGameStatusesAsync(Guid userId)
        {
            var list = await _gameStatusRepository.GetByUserIdAsync(userId);
            var result = _mapper.Map<IEnumerable<GameStatusResponse>>(list);
            return Result<IEnumerable<GameStatusResponse>>.Success(result);
        }
    }
}