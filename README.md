# MyFps · 1인칭 게임 학습 프로젝트

플레이어 이동과 사격 및 적 AI·아이템·장면 연출을 구성한 Unity 1인칭 게임 학습 프로젝트입니다.

## 학습·재구현 대상

- 플레이어 이동과 시점·사격 및 체력 처리
- NavMesh 기반 적 순찰·추격·공격 상태 전환
- 탄약과 열쇠 및 퍼즐 아이템 상호작용
- 시퀀스 트리거와 씬 전환
- 플레이 데이터 저장·불러오기와 메뉴 UI

## 주요 코드와 구조

| 경로 | 역할 |
| --- | --- |
| [Assets/MyFps/Scripts/Player](Assets/MyFps/Scripts/Player) | 플레이어 이동·사격·상호작용 |
| [Assets/MyFps/Scripts/Enemy/GunMan.cs](Assets/MyFps/Scripts/Enemy/GunMan.cs) | 적 상태 전환과 추적 |
| [Assets/MyFps/Scripts/PlayerData](Assets/MyFps/Scripts/PlayerData) | 플레이 데이터와 저장 처리 |
| [Assets/MyFps/Scripts/Sequence](Assets/MyFps/Scripts/Sequence) | 장면 연출과 진행 트리거 |
| [Assets/MyFps/Scenes](Assets/MyFps/Scenes) | 메뉴와 도입 및 플레이 씬 |
| [Assets/FPS](Assets/FPS) | 별도 FPS 관련 코드·리소스 |
| [Assets/_Sample](Assets/_Sample) | 개념별 실습 |

## 실행 방법

1. Unity Hub에서 저장소 폴더를 프로젝트로 추가합니다.
2. **Unity 6000.3.13f1**로 열고 패키지 복원과 임포트를 기다립니다.
3. [시작 씬](Assets/MyFps/Scenes/MainMenu.unity)을 엽니다.
4. Play Mode에서 동작을 확인합니다.

저장·불러오기는 현재 코드의 이진 직렬화 방식을 사용합니다. 저장 형식의 호환성과 손상 파일 처리 등은 별도 검토 대상입니다.

## 학습 방식과 AI 활용

수업 예제를 직접 제작한 뒤 수업에서 사용한 에셋과 관련 자료를 바탕으로 AI를 활용해 같은 기능을 다시 구현하는 학습을 진행했습니다. 수업 실습과 AI를 활용한 재구현 작업이 함께 포함된 학습 과정입니다.

| 구분 | 내용 |
| --- | --- |
| 학습 기반 | 수업에서 다룬 게임 기능과 사용 에셋 |
| AI 활용 | 기존 학습 내용을 바탕으로 기능을 재구현하는 과정에 활용 |
| 자료 성격 | 게임 동작과 구현 구조를 익히기 위한 실습 기록 |

위 기능 목록은 저장소의 코드 구성을 설명합니다. 파일별 수업 예제와 AI 재구현의 구분은 개별 작업 기록을 기준으로 확인합니다.

## 검증과 리소스

실행·빌드 결과는 각 실습 당시의 기록을 기준으로 하며 이 문서의 수정으로 새로운 검증 결과가 추가된 것은 아닙니다. 수업 에셋과 외부 샘플·라이브러리는 각 제공자의 라이선스를 따릅니다.
